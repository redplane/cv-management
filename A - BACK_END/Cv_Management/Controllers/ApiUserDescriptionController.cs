using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.Enums;
using ApiClientShared.Extensions;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel.UserDescription;
using CvManagement.Interfaces.Services;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/user-description")]
    public class ApiUserDescriptionController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="userDescriptionService"></param>
        /// <param name="profileService"></param>
        public ApiUserDescriptionController(
            IUserDescriptionService userDescriptionService,
            IProfileService profileService)
        {
            _userDescriptionService = userDescriptionService;
            _profileService = profileService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Service to handle profile.
        /// </summary>
        private readonly IProfileService _profileService;

        private readonly IUserDescriptionService _userDescriptionService;

        #endregion

        #region  Methods

        /// <summary>
        ///     Get user description using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchUserDescriptionViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchUserDescriptionViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (condition.Ids != null && condition.Ids.Count > 0)
                condition.Ids = condition.Ids.Where(x => x > 0).ToHashSet();

            if (condition.UserIds != null && condition.UserIds.Count > 0)
                condition.UserIds = condition.UserIds.Where(x => x > 0).ToHashSet();

            var loadUserDescriptionResult =
                await _userDescriptionService.SearchUserDescriptionsAsync(condition, CancellationToken.None);
            return Ok(loadUserDescriptionResult);
        }

        /// <summary>
        ///     Create User description
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddUserDescription([FromBody] AddUserDescriptionViewModel model)
        {
            if (model == null)
            {
                model = new AddUserDescriptionViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get user profile.
            var profile = _profileService.GetProfile(Request);
            if (profile.Role != UserRoles.Admin)
                model.UserId = profile.Id;

            var userDescription = await _userDescriptionService.AddUserDescriptionAsync(model);
            return Ok(userDescription);
        }

        /// <summary>
        ///     Update User description
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> EditUserDescription([FromUri] int id,
            [FromBody] EditUserDescriptionViewModel model)
        {
            if (model == null)
            {
                model = new EditUserDescriptionViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userDescription =
                await _userDescriptionService.EditUserDescriptionAsync(id, model, CancellationToken.None);
            return Ok(userDescription);
        }

        /// <summary>
        ///     Delete User Desciption from Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteUserDescription([FromUri] int id)
        {
            await _userDescriptionService.DeleteUserDescriptionAsync(id, CancellationToken.None);
            return Ok();
        }

        #endregion
    }
}