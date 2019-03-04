using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.ViewModel.ProjectResponsibility;
using CvManagement.Interfaces.Services.Businesses;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/project-responsibility")]
    public class ApiProjectResponsibilityController : ApiController
    {
        #region properties

        private readonly IProjectResponsibilityService _projectResponsibilityService;

        #endregion

        #region Contructors

        public ApiProjectResponsibilityController(IProjectResponsibilityService projectResponsibilityService)
        {
            _projectResponsibilityService = projectResponsibilityService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get project responsibility using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectResponsibilityViewModel condition)
        {
            //Check null for model 
            if (condition == null)
            {
                condition = new SearchProjectResponsibilityViewModel();
                Validate(condition);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadProjectResponsibilitiesResult =
                await _projectResponsibilityService.SearchProjectResponsibilitiesAsync(condition);
            return Ok(loadProjectResponsibilitiesResult);
        }

        #endregion
    }
}