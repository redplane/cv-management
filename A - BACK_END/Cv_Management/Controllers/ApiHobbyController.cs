using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Hobby;
using CvManagement.Interfaces.Services;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/hobby")]
    public class ApiHobbyController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="dbService"></param>
        /// <param name="profileService"></param>
        /// <param name="unitOfWork"></param>
        public ApiHobbyController(IDbService dbService, IProfileService profileService, IUnitOfWork unitOfWork)
        {
            _dbService = dbService;
            _profileService = profileService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Properties
        
        /// <summary>
        ///     Service which handles database operation.
        /// </summary>
        private readonly IDbService _dbService;

        private readonly IProfileService _profileService;

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Methods

        /// <summary>
        ///     Get hobbies using specific condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchHobbyViewModel condition)
        {
            //Check model is null
            if (condition == null)
            {
                condition = new SearchHobbyViewModel();
                Validate(condition);
            }
            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hobbies = _unitOfWork.Hobbies.Search();

            //Search for ids
            if (condition.Ids != null && condition.Ids.Any())
            {
                var ids = condition.Ids.Where(c => c > 0).ToList();
                if (ids.Any())
                    hobbies = hobbies.Where(c => ids.Contains(c.Id));
            }

            //Search for UserIds
            if (condition.UserIds != null && condition.UserIds.Any())
            {
                var userIds = condition.UserIds.Where(c => c > 0).ToList();
                if (userIds.Any())
                    hobbies = hobbies.Where(c => userIds.Contains(c.UserId));
            }

            //Search for names
            if (condition.Names != null && condition.Names.Any())
            {
                var names = condition.Names.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (names.Any())
                    hobbies = hobbies.Where(c => names.Contains(c.Name));
            }

            var result = new SearchResultViewModel<IList<Hobby>>();
            result.Total = await hobbies.CountAsync();

            //Do sort
            hobbies = _dbService.Sort(hobbies, SortDirection.Ascending, SkillSortProperty.Id);

            //Do pagination
            hobbies = _dbService.Paginate(hobbies, condition.Pagination);

            result.Records = await hobbies.ToListAsync();

            return Ok(result);
        }

        /// <summary>
        ///     Add an hobby
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddHobby([FromBody] AddHobbyViewModel model)
        {
            //Check null for model
            if (model == null)
            {
                model = new AddHobbyViewModel();
                Validate(model);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get profile information.
            var profile = _profileService.GetProfile(Request);

            var hobby = new Hobby();
            hobby.Name = model.Name;
            hobby.UserId = profile.Role == UserRoles.Admin && model.UserId != null ? model.UserId.Value : profile.Id;
            hobby.Description = model.Description;

            //Add to db context
            _unitOfWork.Hobbies.Insert(hobby);
            await _unitOfWork.CommitAsync();

            return Ok(hobby);
        }

        /// <summary>
        ///     Edit an hobby
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> EditHobby([FromUri] int id, [FromBody] EditHobbyViewModel model)
        {
            //Check null for model
            if (model == null)
            {
                model = new EditHobbyViewModel();
                Validate(model);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Find hobby by id
            var hobbies = _unitOfWork.Hobbies.Search();
            var hobby = await hobbies.FirstOrDefaultAsync(x => x.Id == id);
            if (hobby == null)
                return NotFound();

            //Update hobby
            if (!string.IsNullOrEmpty(model.Name))
                hobby.Name = model.Name;

            if (!string.IsNullOrEmpty(model.Description))
                hobby.Description = model.Description;

            //Save change to database
            await _unitOfWork.CommitAsync();

            return Ok(hobby);
        }

        /// <summary>
        ///     Delete an hobby
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteHobby([FromUri] int id)
        {
            //Find hobby
            var hobbies = _unitOfWork.Hobbies.Search();
            var hobby = await hobbies.FirstOrDefaultAsync(x => x.Id == id);
            if (hobby == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    HttpMessages.HobbyNotFound));

             _unitOfWork.Hobbies.Remove(hobby);

            //Save change to db
            await _unitOfWork.CommitAsync();

            return Ok();
        }

        #endregion
    }
}