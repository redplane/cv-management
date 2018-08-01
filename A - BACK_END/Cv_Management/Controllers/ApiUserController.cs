using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using Cv_Management.Constant;
using Cv_Management.Enums.SortProperties;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models.Entities;
using Cv_Management.Models.Entities.Context;
using Cv_Management.ViewModel;
using Cv_Management.ViewModel.User;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/user")]
    public class ApiUserController : ApiController
    {
        #region Properties

        /// <summary>
        /// Database context to access to database.
        /// </summary>
        private readonly CvManagementDbContext _dbContext;

        /// <summary>
        /// Service to handle database query.
        /// </summary>
        private readonly IDbService _dbService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        public ApiUserController(DbContext dbContext, IDbService dbService)
        {
            _dbContext = (CvManagementDbContext)dbContext;
            _dbService = dbService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get users using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchUserViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchUserViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get users.
            var users = _dbContext.Users.AsQueryable();

            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                var ids = condition.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    users = users.Where(user => ids.Contains(user.Id));
            }

            if (condition.LastNames != null && condition.LastNames.Count > 0)
            {
                var lastNames = condition.LastNames.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                if (lastNames.Count > 0)
                    users = users.Where(user => lastNames.Any(lastName => user.LastName.Contains(lastName)));
            }

            if (condition.FirstNames != null && condition.FirstNames.Count > 0)
            {
                var firstNames = condition.FirstNames.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                if (firstNames.Count > 0)
                    users = users.Where(user => firstNames.Any(firstName => user.FirstName.Contains(firstName)));
            }

            var loadUserResult = new SearchResultViewModel<IList<UserViewModel>>();
            loadUserResult.Total = await users.CountAsync();

            IQueryable<UserViewModel> loadedUsers = null;
            IQueryable<UserDescription> userDescriptions = Enumerable.Empty<UserDescription>().AsQueryable();

            if (condition.IncludeDescriptions)
                userDescriptions = _dbContext.UserDescriptions.AsQueryable();

            loadedUsers = from user in users
                          select new UserViewModel
                          {
                              Id = user.Id,
                              Email = user.Email,
                              FirstName = user.FirstName,
                              LastName = user.LastName,
                              Photo = user.Photo,
                              Birthday = user.Birthday,
                              Role = user.Role,
                              Descriptions = userDescriptions.Where(userDescription => userDescription.UserId == user.Id)
                          };

            // Sort.
            loadedUsers = _dbService.Sort(loadedUsers, SortDirection.Ascending, UserSortProperty.Id);

            // Do pagination.
            loadedUsers = _dbService.Paginate(loadedUsers, condition.Pagination);
            loadUserResult.Records = loadedUsers.ToList();
            return Ok(loadUserResult);
        }

        ///// <summary>
        ///// Create User
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("")]
        //public async Task<IHttpActionResult> Create([FromBody] CreateUserViewModel model)
        //{
        //    if (model == null)
        //    {
        //        model = new CreateUserViewModel();
        //        Validate(model);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var user = new User();
        //    MappingData(user, model);

        //    user = DbSet.Users.Add(user);
        //    await DbSet.SaveChangesAsync();
        //    return Ok(user);
        //}

        ///// <summary>
        ///// Update User
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("{id}")]
        //public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody] UpdateUserViewModel model)
        //{
        //    var user = DbSet.Users.Find(id);
        //    if (user == null)
        //        return NotFound();
        //    if (model == null)
        //    {
        //        model = new UpdateUserViewModel();
        //        Validate(model);
        //    }
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    MappingDataForUpdate(user, model);

        //    await DbSet.SaveChangesAsync();
        //    return Ok(user);
        //}

        ///// <summary>
        ///// Delete User using Id
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpDelete]
        //[Route("{id}")]
        //public IHttpActionResult Delete([FromUri]int id)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Mapping data from Entity to model for create
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <param name="model"></param>
        //public void MappingData(User entity, CreateUserViewModel model)
        //{
        //    entity.FirstName = model.FirstName;
        //    entity.LastName = model.LastName;
        //    entity.Birthday = model.Birthday;
        //    if (model.Photo != null)
        //        entity.Photo = Convert.ToBase64String(model.Photo.Buffer);
        //    entity.Email = model.Email;
        //    entity.Password = model.Password;
        //}

        ///// <summary>
        ///// Mapping data from entity to model for update
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <param name="model"></param>
        //public void MappingDataForUpdate(User entity, UpdateUserViewModel model)
        //{
        //    if (!string.IsNullOrEmpty(model.FirstName))
        //        entity.FirstName = model.FirstName;
        //    if (!string.IsNullOrEmpty(model.LastName))
        //        entity.LastName = model.LastName;
        //    if (model.Birthday != null)
        //        entity.Birthday = model.Birthday.GetValueOrDefault();

        //}


        ///// <summary>
        ///// Get personal info from token
        ///// </summary>
        ///// <param name="accessToken"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("personal-profile/{accessToken}")]
        //public async Task<IHttpActionResult> GetPersonalInfo([FromUri]string accessToken)
        //{
        //    try
        //    {
        //        IJsonSerializer serializer = new JsonNetSerializer();
        //        IDateTimeProvider provider = new UtcDateTimeProvider();
        //        IJwtValidator validator = new JwtValidator(serializer, provider);
        //        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        //        IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

        //        var json = decoder.Decode(accessToken, GlobalConstant.Secret, verify: true);
        //        var account = JsonConvert.DeserializeObject<AcountViewModel>(json);
        //        var user = await DbSet.Users.FirstOrDefaultAsync(c =>
        //            c.Email == account.Username && c.Password == account.Password);
        //        if (user == null)
        //            return NotFound();
        //        return Ok(user);
        //    }
        //    catch (TokenExpiredException e)
        //    {
        //        throw new Exception("TOKEN_HAS_EXPRIED");

        //    }

        //}

        //#region Login
        ///// <summary>
        ///// Login 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("login")]
        //public async Task<IHttpActionResult> Login([FromBody] LoginViewModel model)
        //{
        //    if (model == null)
        //    {
        //        model = new LoginViewModel();
        //        Validate(model);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    //get user by username and password
        //    var user = await DbSet.Users.FirstOrDefaultAsync(c =>
        //        c.Email.Equals(model.Username) && c.Password.Equals(model.Password));
        //    if (user == null)
        //        return NotFound();

        //    var result = new ResponsesLoginViewModel();
        //    result.LifeTime = 3600;
        //    result.AccessToken = GetToken(user);
        //    result.Type = "Bearer";
        //    return Ok(result);
        //}

        ///// <summary>
        ///// Get token using web token
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public string GetToken(User user)
        //{
        //    var userToken = new AcountViewModel()
        //    {
        //        Username = user.Email,
        //        Password = user.Password,

        //    };

        //    IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        //    IJsonSerializer serializer = new JsonNetSerializer();
        //    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        //    IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

        //    string token = encoder.Encode(userToken, GlobalConstant.Secret);
        //    return token;
        //}
        //#endregion

        //#region Register
        ///// <summary>
        ///// Register new user
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("register")]
        //public async Task<IHttpActionResult> Register([FromBody]RegisterViewModel model)
        //{
        //    if (model == null)
        //    {
        //        model = new RegisterViewModel();
        //        Validate(model);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    //check duplicate
        //    var isDuplicate = await DbSet.Users.AnyAsync(c => c.Email == model.Email && c.Password == model.Password);
        //    if (isDuplicate)
        //        return Conflict();

        //    var user = new User();
        //    user.Email = model.Email;
        //    user.LastName = model.LastName;
        //    user.FirstName = model.FirstName;
        //    user.Password = model.Password;

        //    DbSet.Users.Add(user);
        //    await DbSet.SaveChangesAsync();
        //    return Ok(user);
        //}

        //#endregion
        #endregion
    }
}