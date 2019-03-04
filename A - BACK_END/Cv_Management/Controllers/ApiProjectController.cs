using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.ViewModel.Project;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/project")]
    public class ApiProjectController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors
        /// </summary>
        /// <param name="projectService"></param>
        public ApiProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        #endregion

        #region Properties

        private readonly IProjectService _projectService;

        #endregion


        #region Methods

        /// <summary>
        ///     Get projects using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchProjectViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadProjectsResult = await _projectService.SearchProjectsAsync(condition, CancellationToken.None);
            return Ok(loadProjectsResult);
        }

        /// <summary>
        ///     Create Project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddProject([FromBody] AddProjectViewModel model)
        {
            if (model == null)
            {
                model = new AddProjectViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = await _projectService.AddProjectAsync(model, CancellationToken.None);
            return Ok(project);
        }

        /// <summary>
        ///     Update Project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody] EditProjectViewModel model)
        {
            if (model == null)
            {
                model = new EditProjectViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = await _projectService.EditProjectAsync(id, model, CancellationToken.None);
            return Ok(project);
        }

        /// <summary>
        ///     Delete project using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete([FromUri] int id)
        {
            await _projectService.DeleteProjectAsync(id, CancellationToken.None);
            return Ok();
        }

        #endregion
    }
}