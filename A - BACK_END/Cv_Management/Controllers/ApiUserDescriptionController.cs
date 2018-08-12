using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.UserDescription;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/user-description")]
    public class ApiUserDescriptionController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        /// <param name="profileService"></param>
        public ApiUserDescriptionController(DbContext dbContext, IDbService dbService, IProfileService profileService)
        {
            _dbContext = (CvManagementDbContext) dbContext;
            _dbService = dbService;
            _profileService = profileService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Context to access to database.
        /// </summary>
        private readonly CvManagementDbContext _dbContext;

        /// <summary>
        ///     Service to handle database operation.
        /// </summary>
        private readonly IDbService _dbService;

        /// <summary>
        ///     Service to handle profile.
        /// </summary>
        private readonly IProfileService _profileService;

        #endregion

        #region  Methods

        /// <summary>
        ///     Get user description using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchUserDescriptionViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchUserDescriptionViewModel();
                Validate(condition);
            }

            if (ModelState.IsValid)
                return BadRequest(ModelState);

            var userDescriptions = _dbContext.UserDescriptions.AsQueryable();

            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                var ids = condition.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    userDescriptions = userDescriptions.Where(userDescription => ids.Contains(userDescription.Id));
            }

            if (condition.UserIds != null && condition.UserIds.Count > 0)
            {
                var userIds = condition.UserIds.Where(x => x > 0).ToList();
                if (userIds.Count > 0)
                    userDescriptions =
                        userDescriptions.Where(userDescription => userIds.Contains(userDescription.UserId));
            }

            var loadUserDescriptionResult = new SearchResultViewModel<IList<UserDescription>>();
            loadUserDescriptionResult.Total = await userDescriptions.CountAsync();

            // Do sorting.
            userDescriptions =
                _dbService.Sort(userDescriptions, SortDirection.Ascending, UserDescriptionSortProperty.Id);

            // Do pagination.
            userDescriptions = _dbService.Paginate(userDescriptions, condition.Pagination);

            loadUserDescriptionResult.Records = await userDescriptions.ToListAsync();
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

            // Add user description into database.
            var userDescription = new UserDescription();
            userDescription.UserId = profile.Id;
            userDescription.Description = model.Description;

            // Add the description into database.
            userDescription = _dbContext.UserDescriptions.Add(userDescription);

            // Save changes into database.
            await _dbContext.SaveChangesAsync();
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

            // Find the user description by using id.
            var userDescriptions = _dbContext.UserDescriptions.AsQueryable();
            var userDescription = await userDescriptions.FirstOrDefaultAsync(x => x.Id == id);

            // Find the first record.
            if (userDescription == null)
                return NotFound();

            userDescription.Description = model.Description;

            // Save changes.
            await _dbContext.SaveChangesAsync();
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
            // Find the user description.
            var userDescriptions = _dbContext.UserDescriptions;

            // Find the user description in the database.
            var userDescription = await userDescriptions.FirstOrDefaultAsync(x => x.Id == id);
            if (userDescription == null)
                return NotFound();

            // Delete the description from database.
            _dbContext.UserDescriptions.Remove(userDescription);

            // Save changes in database.
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        #endregion
    }
}