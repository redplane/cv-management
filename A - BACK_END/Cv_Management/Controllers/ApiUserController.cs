using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Hobby;
using ApiClientShared.ViewModel.User;
using ApiClientShared.ViewModel.UserDescription;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using Cv_Management.Constant;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/user")]
    public class ApiUserController : ApiController
    {
        #region Properties
        /// <summary>
        /// Context to access to database
        /// </summary>
        private readonly CvManagementDbContext _dbContext;

        /// <summary>
        /// Service to handler database operation
        /// </summary>
        private readonly IDbService _dbService;

        #endregion

        #region Contructors
        /// <summary>
        /// Initalize controller with Injectors
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        public ApiUserController(DbContext dbContext,
            IDbService dbService)
        {
            _dbContext = (CvManagementDbContext)dbContext;
            _dbService = dbService;

        }
        #endregion

        #region Methods

        /// <summary>
        /// Get users using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [Route("search")]
        [HttpPost]
        public async Task<IHttpActionResult> Search([FromBody] SearchUserViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchUserViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var users = _dbContext.Users.AsQueryable();

            if (condition.Ids != null)
            {
                var ids = condition.Ids.Where(c => c > 0).ToList();
                if (ids.Count > 0)
                    users = users.Where(c => condition.Ids.Contains(c.Id));
            }

            if (condition.LastNames != null)
            {
                var lastNames = condition.LastNames.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (lastNames.Count > 0)
                    users = users.Where(c => lastNames.Contains(c.LastName));
            }

            if (condition.FirstNames != null)
            {
                var firstNames = condition.FirstNames.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (firstNames.Count > 0)
                    users = users.Where(c => firstNames.Contains(c.FirstName));
            }

            if (condition.Birthday != null)
            {
                var birthday = condition.Birthday;
                if (birthday.From != null)
                    users = users.Where(c => c.Birthday >= birthday.From);

                if (birthday.To != null)
                    users = users.Where(user => user.Birthday <= birthday.To);
            }

            #region Search user descriptions && hobbies

            //user descriptions
            IQueryable<UserDescription> userDescriptions = Enumerable.Empty<UserDescription>().AsQueryable();

            if (condition.IncludeDescriptions)
                userDescriptions = _dbContext.UserDescriptions.AsQueryable();

            // Get all hobbies.
            IQueryable<Hobby> hobbies = Enumerable.Empty<Hobby>().AsQueryable();
            if (condition.IncludeHobbies)
                hobbies = _dbContext.Hobbies.AsQueryable();

            var loadedUsers = from user in users
                              select new UserViewModel
                              {
                                  Id = user.Id,
                                  Birthday = user.Birthday,
                                  Email = user.Email,
                                  FirstName = user.FirstName,
                                  LastName = user.LastName,
                                  Photo = user.Photo,
                                  Role = user.Role,
                                  Descriptions = from description in userDescriptions
                                                 select new UserDescriptionViewModel
                                                 {
                                                     Id = description.Id,
                                                     Description = description.Description,
                                                     UserId = description.UserId
                                                 },
                                  Hobbies = from hobby in hobbies
                                            select new HobbyViewModel
                                            {
                                                Id = hobby.Id,
                                                Name = hobby.Name,
                                                UserId = hobby.UserId,
                                                Description = hobby.Description
                                            }
                              };
            #endregion

            var result = new SearchResultViewModel<IList<UserViewModel>>();
            result.Total = await users.CountAsync();

            // Do sort
            loadedUsers = _dbService.Sort(loadedUsers, SortDirection.Ascending, UserSortProperty.Id);

            // Do pagination
            loadedUsers = _dbService.Paginate(loadedUsers, condition.Pagination);

            result.Records = await loadedUsers.ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Add an user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser([FromBody] AddUserViewModel model)
        {
            if (model == null)
            {
                model = new AddUserViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Birthday = model.Birthday;
            if (model.Photo != null)
                user.Photo = Convert.ToBase64String(model.Photo.Buffer);
            user.Role = model.Role;
            user.Email = model.Email;
            user.Password = model.Password;

            user = _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok(user);
        }


        /// <summary>
        /// Edit an user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> EditUser([FromUri]int id, [FromBody] EditUserViewModel model)
        {
            //validate model
            if (model == null)
            {
                model = new EditUserViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Find user
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (model.Birthday != null)
                user.Birthday = model.Birthday.Value;

            if (model.Photo != null)
                user.Photo = Convert.ToBase64String(model.Photo.Buffer);

            //Save to database
            await _dbContext.SaveChangesAsync();

            return Ok(user);
        }


        /// <summary>
        /// Delete an user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteUser([FromUri] int id)
        {
            //Find user by id
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            //Remove user
            _dbContext.Users.Remove(user);

            //save change to database
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginViewModel model)
        {
            if (model == null)
            {
                model = new LoginViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //get user by username and password
            var user = await _dbContext.Users.FirstOrDefaultAsync(c =>
                c.Email.Equals(model.Email) && c.Password.Equals(model.Password));
            if (user == null)
                return NotFound();

            var result = new TokenViewModel();
            result.LifeTime = 3600;
            result.AccessToken = GetToken(user);
            result.Type = "Bearer";
            return Ok(result);
        }

        /// <summary>
        /// Get token using web token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetToken(User user)
        {
            var userToken = new AcountViewModel()
            {
                Username = user.Email,
                Password = user.Password,
                Role = user.Role

            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            string token = encoder.Encode(userToken, GlobalConstant.Secret);
            return token;
        }


        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (model == null)
            {
                model = new RegisterViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //check duplicate
            var isDuplicate = await _dbContext.Users.AnyAsync(c => c.Email == model.Email && c.Password == model.Password);
            if (isDuplicate)
                return Conflict();

            var user = new User();
            user.Email = model.Email;
            user.LastName = model.LastName;
            user.FirstName = model.FirstName;
            user.Password = model.Password;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok(user);
        }

        #endregion
    }
}