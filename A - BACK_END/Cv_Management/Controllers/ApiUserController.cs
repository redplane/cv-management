using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Constants;
using ApiClientShared.Enums;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Hobby;
using ApiClientShared.ViewModel.User;
using ApiClientShared.ViewModel.UserDescription;
using AutoMapper;
using Cv_Management.Attributes;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using Cv_Management.Services.CacheServices;
using Cv_Management.ViewModels;
using Cv_Management.ViewModels.User;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;
using Microsoft.EntityFrameworkCore;

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
        /// <param name="fileService"></param>
        /// <param name="profileCacheService"></param>
        /// <param name="userService"></param>
        /// <param name="mapper"></param>
        /// <param name="appPath"></param>
        public ApiUserController(DbContext dbContext,
            IDbService dbService,
            ITokenService tokenService, IProfileService profileService,
            ICaptchaService captchaService, IFileService fileService,
            IValueCacheService<string, ProfileModel> profileCacheService,
            IUserService userService,
            IMapper mapper,
            AppPathModel appPath)
        {
            _dbContext = (BaseCvManagementDbContext)dbContext;
            _dbService = dbService;
            _tokenService = tokenService;
            _profileService = profileService;
            _captchaService = captchaService;
            _fileService = fileService;
            _profileCacheService = profileCacheService;
            _userService = userService;
            _mapper = mapper;
            _appPath = appPath;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Context to access to database
        /// </summary>
        private readonly BaseCvManagementDbContext _dbContext;

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

        /// <summary>
        /// Service for handling file operation.
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// Profile cache service.
        /// </summary>
        private readonly IValueCacheService<string, ProfileModel> _profileCacheService;

        /// <summary>
        /// Service to handle user data.
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Automapper DI.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Application path configuration.
        /// </summary>
        private readonly AppPathModel _appPath;

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
            
            var result = new SearchResultViewModel<IList<User>>();
            result.Total = await users.CountAsync();

            // Do sort
            users = _dbService.Sort(users, SortDirection.Ascending, UserSortProperty.Id);

            // Do pagination
            users = _dbService.Paginate(users, condition.Pagination);

            result.Records = await users.ToListAsync();

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

            _dbContext.Users.Add(user);
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
            #region Model validation

            if (model == null)
            {
                model = new EditUserViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Profile image validation

            var photo = _fileService.GetImage(model.Photo?.Buffer);
            if (photo != null)
            {
                if (photo.Width != ImageSizeConstant.StandardProfileImageSize ||
                    photo.Height != ImageSizeConstant.StandardProfileImageSize)
                {
                    ModelState.AddModelError($"{nameof(model)}.{nameof(model.Photo)}", HttpMessages.ProfileImageSizeInvalid);
                    return BadRequest(ModelState);
                }
            }

            #endregion

            //Find user
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, HttpMessages.UserNotFound));

            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (model.Birthday != null)
                user.Birthday = model.Birthday.Value;

            // Photo is defined. Save photo to path.
            if (model.Photo != null)
            {
                var relativeProfileImagePath = await _fileService.AddFileToDirectory(model.Photo.Buffer, _appPath.ProfileImage, null, CancellationToken.None);
                user.Photo = Url.Content(relativeProfileImagePath);
                ;
            }

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
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.OK, HttpMessages.UserNotFound));

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

            // Get profile from system.
            var profile = await _userService.LoginAsync(model.Email, model.Password, CancellationToken.None);

            ;            // User is not found.
            if (profile == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, HttpMessages.UserNotFound));

            // Initialize access token.
            var token = new TokenViewModel();
            token.LifeTime = 3600;
            token.Type = "Bearer";

            if (string.IsNullOrWhiteSpace(profile.AccessToken))
            {
                // Add expired time.
                var expiredAt = DateTime.UtcNow.AddSeconds(token.LifeTime);

                var payload = new Dictionary<string, string>();
                payload.Add(ClaimTypes.Email, profile.Email);
                payload.Add(ClaimTypes.Name, $"{profile.FirstName} {profile.LastName}");
                payload.Add(ClaimTypes.Expired, expiredAt.ToString("yyyy/MM/dd"));
                profile.AccessToken = token.AccessToken = _tokenService.Encode(payload);
                _profileCacheService.Add(profile.Email, profile, token.LifeTime);
            }
            else
                token.AccessToken = profile.AccessToken;

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
            #region Model validation

            if (model == null)
            {
                model = new RegisterViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

#if !BY_PASS_CAPTCHA
            // Verify the capcha first.
            var bIsCaptchaValid = await _captchaService.IsCaptchaValidAsync(model.ClientCaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
            {
                ModelState.AddModelError($"{nameof(model)}.{nameof(model.ClientCaptchaCode)}", HttpMessages.CaptchaInvalid);
                return BadRequest(ModelState);
            }
#endif

            #endregion

            #region Find user duplicated

            var users = _dbContext.Users.AsQueryable();
            var user = await users.FirstOrDefaultAsync(x => x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));
            if (user != null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict,
                    HttpMessages.RegistrationDuplicate));

            #endregion

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                #region Initialize user

                user = new User();
                user.Email = model.Email;
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;

                // Mark account as pending.
                user.Status = UserStatuses.Pending;

                // Hash the password for user protection.
                user.Password = _profileService.HashPassword(model.Password);

                _dbContext.Users.Add(user);

                #endregion

                #region Initialize token

                // Find list of profile activation tokens.
                var profileActivationTokens = _dbContext.ProfileActivationTokens.AsQueryable();
                profileActivationTokens = profileActivationTokens.Where(x =>
                    x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));

                var profileActivationToken = new ProfileActivationToken();
                profileActivationToken.Email = model.Email;
                profileActivationToken.Token = Guid.NewGuid().ToString("D");
                profileActivationToken.CreatedTime = 0;

                // Delete previous activation token and add the new one.
                _dbContext.ProfileActivationTokens.RemoveRange(profileActivationTokens);
                _dbContext.ProfileActivationTokens.Add(profileActivationToken);

                #endregion

                // TODO: Send activation email.

                await _dbContext.SaveChangesAsync();
                transactionScope.Complete();
            }


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

                profile.Password = null;
                profile.AccessToken = null;
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