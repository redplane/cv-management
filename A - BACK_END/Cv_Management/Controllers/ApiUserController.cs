﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Hobby;
using ApiClientShared.ViewModel.User;
using ApiClientShared.ViewModel.UserDescription;
using Cv_Management.Attributes;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/user")]
    public class ApiUserController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initalize controller with Injectors
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        /// <param name="tokenService"></param>
        /// <param name="profileService"></param>
        /// <param name="captchaService"></param>
        public ApiUserController(DbContext dbContext,
            IDbService dbService,
            ITokenService tokenService, IProfileService profileService, 
            ICaptchaService captchaService)
        {
            _dbContext = (CvManagementDbContext)dbContext;
            _dbService = dbService;
            _tokenService = tokenService;
            _profileService = profileService;
            _captchaService = captchaService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Context to access to database
        /// </summary>
        private readonly CvManagementDbContext _dbContext;

        /// <summary>
        ///     Service to handler database operation
        /// </summary>
        private readonly IDbService _dbService;

        /// <summary>
        /// Service which is for handling token generation.
        /// </summary>
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Service which handles profile operation.
        /// </summary>
        private readonly IProfileService _profileService;

        /// <summary>
        /// Service for verifying captcha code.
        /// </summary>
        private readonly ICaptchaService _captchaService;

        #endregion

        #region Methods

        /// <summary>
        ///     Get users using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [Route("search")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchUserViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchUserViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get list of users.
            var users = _dbContext.Users.AsQueryable();

            // Ids are defined.
            if (condition.Ids != null)
            {
                var ids = condition.Ids.Where(c => c > 0).ToList();
                if (ids.Count > 0)
                    users = users.Where(c => condition.Ids.Contains(c.Id));
            }

            // Last names are defined.
            if (condition.LastNames != null)
            {
                var lastNames = condition.LastNames.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (lastNames.Count > 0)
                    users = users.Where(c => lastNames.Contains(c.LastName));
            }

            // First names are defined.
            if (condition.FirstNames != null)
            {
                var firstNames = condition.FirstNames.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (firstNames.Count > 0)
                    users = users.Where(c => firstNames.Contains(c.FirstName));
            }

            // Birthday range is defined.
            if (condition.Birthday != null)
            {
                var birthday = condition.Birthday;
                if (birthday.From != null)
                    users = users.Where(c => c.Birthday >= birthday.From);

                if (birthday.To != null)
                    users = users.Where(user => user.Birthday <= birthday.To);
            }
            
            // Get request profile.
            var profile = _profileService.GetProfile(Request);

            // User is anonymous or ordinary.
            if (profile == null || profile.Role != UserRoles.Admin)
            {
                // Statuses are defined.
                if (condition.Statuses != null)
                {
                    var statuses = condition.Statuses.Where(x => Enum.IsDefined(typeof(UserRoles), x)).ToList();
                    if (statuses.Count > 0)
                        users = users.Where(x => statuses.Contains(x.Status));
                }

                // Roles are defined.
                if (condition.Roles != null)
                {
                    var roles = condition.Roles.Where(x => Enum.IsDefined(typeof(UserRoles), x)).ToList();
                    if (roles.Count > 0)
                        users = users.Where(x => roles.Contains(x.Role));
                }
            }
            else
                users = users.Where(x => x.Status == UserStatuses.Active);
            
            #region Search user descriptions && hobbies

            //user descriptions
            var userDescriptions = Enumerable.Empty<UserDescription>().AsQueryable();

            if (condition.IncludeDescriptions)
                userDescriptions = _dbContext.UserDescriptions.AsQueryable();

            // Get all hobbies.
            var hobbies = Enumerable.Empty<Hobby>().AsQueryable();
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
        ///     Add an user
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
        ///     Edit an user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> EditUser([FromUri] int id, [FromBody] EditUserViewModel model)
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
        ///     Delete an user
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

            //Save change to database
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        ///     Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login([FromBody] LoginViewModel model)
        {
            if (model == null)
            {
                model = new LoginViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

#if !BY_PASS_CAPTCHA
            // Verify the capcha first.
            var bIsCaptchaValid = await _captchaService.IsCaptchaValidAsync(model.ClientCaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                    HttpMessages.CaptchaInvalid));

#endif
            // Hash the input password.
            var hashedPassword = _profileService.HashPassword(model.Password);

            // Get user by username and password
            var user = await _dbContext.Users.FirstOrDefaultAsync(x =>
                x.Email.Equals(model.Email) && x.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase) && x.Status == UserStatuses.Active);

            // User is not found.
            if (user == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, HttpMessages.UserNotFound));

            var token = new TokenViewModel();
            token.LifeTime = 3600;

            var payload = new Dictionary<string, string>();
            payload.Add(ClaimTypes.Email, user.Email);
            payload.Add(ClaimTypes.Name, $"{user.FirstName} {user.LastName}");

            token.AccessToken = _tokenService.Encode(payload);
            token.Type = "Bearer";
            return Ok(token);
        }
        
        /// <summary>
        ///     Register new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (model == null)
            {
                model = new RegisterViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check duplicate
            var isDuplicate =
                await _dbContext.Users.AnyAsync(c => c.Email == model.Email && c.Password == model.Password);
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

        /// <summary>
        /// Get profile by using user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("personal-profile/{id}")]
        public async Task<IHttpActionResult> GetProfile([FromUri] int? id)
        {
            // Get profile from request.
            var profile = _profileService.GetProfile(Request);

            // No profile is found.
            if (id == null || id < 1)
            {
                if (profile == null)
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        HttpMessages.ProfileNotFound));

                return Ok(profile);
            }

            // Find profile.
            var users = _dbContext.Users.AsQueryable();
            users = users.Where(x => x.Id == id);
            if (profile.Role != UserRoles.Admin)
                users = users.Where(x => x.Status == UserStatuses.Active);

            // Find the first matched record.
            var user = await users.FirstOrDefaultAsync();
            if (user == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    HttpMessages.ProfileNotFound));

            return Ok(user);
        }

        #endregion
    }
}