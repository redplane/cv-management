using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ApiClientShared.Constants;
using ApiClientShared.Enums;
using ApiClientShared.Extensions;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel.User;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using CvManagement.Models;
using CvManagement.ViewModels;
using CvManagement.ViewModels.User;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/user")]
    public class ApiUserController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initalize controller with Injectors
        /// </summary>
        /// <param name="tokenService"></param>
        /// <param name="profileService"></param>
        /// <param name="captchaService"></param>
        /// <param name="fileService"></param>
        /// <param name="profileCacheService"></param>
        /// <param name="userService"></param>
        public ApiUserController(
            ITokenService tokenService, IProfileService profileService,
            ICaptchaService captchaService, IFileService fileService,
            IValueCacheService<string, ProfileModel> profileCacheService,
            IUserService userService)
        {
            _tokenService = tokenService;
            _profileService = profileService;
            _captchaService = captchaService;
            _fileService = fileService;
            _profileCacheService = profileCacheService;
            _userService = userService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Service which is for handling token generation.
        /// </summary>
        private readonly ITokenService _tokenService;

        /// <summary>
        ///     Service which handles profile operation.
        /// </summary>
        private readonly IProfileService _profileService;

        /// <summary>
        ///     Service for verifying captcha code.
        /// </summary>
        private readonly ICaptchaService _captchaService;

        /// <summary>
        ///     Service for handling file operation.
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        ///     Profile cache service.
        /// </summary>
        private readonly IValueCacheService<string, ProfileModel> _profileCacheService;

        /// <summary>
        ///     Service to handle user data.
        /// </summary>
        private readonly IUserService _userService;

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

            // Ids are defined.
            if (condition.Ids != null)
                condition.Ids = condition.Ids.Where(x => x > 0).ToHashSet();

            // Last names are defined.
            if (condition.LastNames != null)
                condition.LastNames = condition.LastNames.Where(x => !string.IsNullOrEmpty(x)).ToHashSet();

            // First names are defined.
            if (condition.FirstNames != null)
                condition.FirstNames = condition.FirstNames.Where(x => !string.IsNullOrEmpty(x)).ToHashSet();

            // Get request profile.
            var profile = _profileService.GetProfile(Request);

            // User is anonymous or ordinary.
            if (profile == null || profile.Role != UserRoles.Admin)
            {
                condition.Statuses = new HashSet<UserStatuses>();
                condition.Statuses.Add(UserStatuses.Active);
            }
            else
            {
                // Statuses are defined.
                if (condition.Statuses != null)
                    condition.Statuses =
                        condition.Statuses.Where(x => Enum.IsDefined(typeof(UserRoles), x)).ToHashSet();

                // Roles are defined.
                if (condition.Roles != null)
                    condition.Roles = condition.Roles.Where(x => Enum.IsDefined(typeof(UserRoles), x)).ToHashSet();
            }

            var searchResult = await _userService.GetUsersAsync(condition);
            return Ok(searchResult);
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

            var user = await _userService.AddUserAsync(model, CancellationToken.None);
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
                if (photo.Width != ImageSizeConstant.StandardProfileImageSize ||
                    photo.Height != ImageSizeConstant.StandardProfileImageSize)
                {
                    ModelState.AddModelError($"{nameof(model)}.{nameof(model.Photo)}",
                        HttpMessages.ProfileImageSizeInvalid);
                    return BadRequest(ModelState);
                }

            #endregion

            var user = await _userService.EditUserAsync(id, model);
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
            // Find user by id
            await _userService.DeleteUserAsync(id, CancellationToken.None);
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
            
            // Verify the capcha first.
            var bIsCaptchaValid =
                await _captchaService.IsCaptchaValidAsync(model.ClientCaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                    HttpMessages.CaptchaInvalid));

            // Get profile from system.
            var profile = await _userService.LoginAsync(model, CancellationToken.None);

            ; // User is not found.
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
            {
                token.AccessToken = profile.AccessToken;
            }

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

            var bIsCaptchaValid =
                await _captchaService.IsCaptchaValidAsync(model.ClientCaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
            {
                ModelState.AddModelError($"{nameof(model)}.{nameof(model.ClientCaptchaCode)}",
                    HttpMessages.CaptchaInvalid);
                return BadRequest(ModelState);
            }

            #endregion

            await _userService.RegisterUserAsync(model, CancellationToken.None);

            // TODO: Send activation email.

            return Ok();
        }

        /// <summary>
        ///     Get profile by using user id.
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
            var conditions = new SearchUserViewModel();
            conditions.Ids = new HashSet<int>();
            conditions.Ids.Add(id.Value);

            if (profile.Role != UserRoles.Admin)
            {
                conditions.Statuses = new HashSet<UserStatuses>();
                conditions.Statuses.Add(UserStatuses.Active);
            }

            // Find the first matched record.
            var user = await _userService.GetUserAsync(conditions);
            if (user == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    HttpMessages.ProfileNotFound));

            return Ok(user);
        }

        #endregion
    }
}