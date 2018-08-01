using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ApiMultiPartFormData.Models;
using Cv_Management.Constant;
using Cv_Management.Entities;
using Cv_Management.Entities.Context;
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

        public readonly DbCvManagementContext DbSet;

        #endregion

        #region Constructors

        public ApiUserController()
        {
            DbSet = new DbCvManagementContext();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get users using specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> GellAll([FromBody] SearchUserViewModel model)
        {

            var users = DbSet.Users.AsQueryable();
            model = model ?? new SearchUserViewModel();
            if (model.Ids != null)
            {
                var ids = model.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    users = users.Where(x => ids.Contains(x.Id));

            }

            if (model.LastNames != null)
                users = users.Where(c => model.LastNames.Contains(c.LastName));

            if (model.FirstNames != null)
                users = users.Where(c => model.FirstNames.Contains(c.FirstName));

            if (model.Birthday > 0)
                users = users.Where(c => c.Birthday.Equals(model.Birthday));

            var results = new SearchResultViewModel<IList<User>>();
            results.Total = await users.CountAsync();
            var pagination = model.Pagination;
            if (pagination != null)
            {
                if (pagination.Page < 1)
                    pagination.Page = 1;
                users = users.Skip((pagination.Page - 1) * pagination.Records)
                    .Take(pagination.Records);
            }
            results.Records = await users.ToListAsync();
            return Ok(results);
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody] CreateUserViewModel model)
        {
            if (model == null)
            {
                model = new CreateUserViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User();
            MappingData(user, model);

            user = DbSet.Users.Add(user);
            await DbSet.SaveChangesAsync();
            return Ok(user);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody] UpdateUserViewModel model)
        {
            var user = DbSet.Users.Find(id);
            if (user == null)
                return NotFound();
            if (model == null)
            {
                model = new UpdateUserViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            MappingDataForUpdate(user, model);

            await DbSet.SaveChangesAsync();
            return Ok(user);
        }

        /// <summary>
        /// Delete User using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete([FromUri]int id)
        {
            return Ok();
        }

        /// <summary>
        /// Mapping data from Entity to model for create
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        public void MappingData(User entity, CreateUserViewModel model)
        {
            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;
            entity.Birthday = model.Birthday;
            if (model.Photo != null)
                entity.Photo = Convert.ToBase64String(model.Photo.Buffer);
            entity.Role = model.Role;
            entity.Email = model.Email;
            entity.Password = model.Password;
        }

        /// <summary>
        /// Mapping data from entity to model for update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        public void MappingDataForUpdate(User entity, UpdateUserViewModel model)
        {
            if (!string.IsNullOrEmpty(model.FirstName))
                entity.FirstName = model.FirstName;
            if (!string.IsNullOrEmpty(model.LastName))
                entity.LastName = model.LastName;
            if (model.Birthday != null)
                entity.Birthday = model.Birthday.GetValueOrDefault();

        }


        /// <summary>
        /// Get personal info from token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("personal-profile/{accessToken}")]
        public async Task<IHttpActionResult> GetPersonalInfo([FromUri]string accessToken)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(accessToken, GlobalConstant.Secret, verify: true);
                var account = JsonConvert.DeserializeObject<AcountViewModel>(json);
                var user = await DbSet.Users.FirstOrDefaultAsync(c =>
                    c.Email == account.Username && c.Password == account.Password);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (TokenExpiredException e)
            {
                throw new Exception("TOKEN_HAS_EXPRIED");

            }

        }

        /// <summary>
        /// Upload file for user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("profile-image/{id}")]
        public async Task<IHttpActionResult> UploadProfile([FromUri] int id, HttpFile photo)
        {
            if (photo == null)
            {
                return BadRequest("No_FILE_CHOOSED");
            }

            var user = await DbSet.Users.FindAsync(id);
            if (user == null)
                return Conflict();

            user.Photo = Convert.ToBase64String(photo.Buffer);
             await DbSet.SaveChangesAsync();

            return Ok(user);
        }

        #region Login
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
            var user = await DbSet.Users.FirstOrDefaultAsync(c =>
                c.Email.Equals(model.Username) && c.Password.Equals(model.Password));
            if (user == null)
                return NotFound();

            var result = new ResponsesLoginViewModel();
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
        #endregion

        #region Register
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
            var isDuplicate = await DbSet.Users.AnyAsync(c => c.Email == model.Email && c.Password == model.Password);
            if (isDuplicate)
                return Conflict();

            var user = new User();
            user.Email = model.Email;
            user.LastName = model.LastName;
            user.FirstName = model.FirstName;
            user.Password = model.Password;

            DbSet.Users.Add(user);
            await DbSet.SaveChangesAsync();
            return Ok(user);
        }

        #endregion
        #endregion
    }
}