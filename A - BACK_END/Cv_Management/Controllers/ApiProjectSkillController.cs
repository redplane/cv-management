using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.ViewModel.ProjectSkill;
using CvManagement.Interfaces.Services.Businesses;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/project-skill")]
    public class ApiProjectSkillController : ApiController
    {
        #region Properties

        private readonly IProjectSkillService _projectSkillService;

        #endregion

        #region Contructors

        public ApiProjectSkillController(IProjectSkillService projectSkillService)
        {
            _projectSkillService = projectSkillService;
        }

        #endregion

        #region Mothods

        /// <summary>
        ///     get projects skill using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectSkillViewModel condition)
        {
            //Check null for condition model
            if (condition == null)
            {
                condition = new SearchProjectSkillViewModel();
                Validate(condition);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var loadProjectSkillsResult = await _projectSkillService.SearchProjectSkillsAsync(condition);
            return Ok(loadProjectSkillsResult);
        }

        #endregion
    }
}