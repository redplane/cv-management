using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Project;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/projet")]
    public class ApiProjectController : ApiController
    {
        #region Properties
        /// <summary>
        /// Context to access to database
        /// </summary>
        public readonly CvManagementDbContext _dbContext;


        /// <summary>
        /// Service to handler controller operation
        /// </summary>
        public readonly IDbService _dbService;

        #endregion

        #region Contructors

        /// <summary>
        /// Initialize controller with injectors
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        public ApiProjectController( CvManagementDbContext dbContext,
            IDbService dbService)
        {
            _dbContext = dbContext;
            _dbService = dbService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get projects using specific conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectViewModel condition)
        {
            if(condition == null )
            {
                condition = new SearchProjectViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var projects = _dbContext.Projects.AsQueryable();
            if (condition.Ids != null)
            {
                var ids = condition.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    projects = projects.Where(x => ids.Contains(x.Id));
            }
           
          if(condition.Names != null)
            {
                var names = condition.Names.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (names.Count > 0)
                    projects = projects.Where(c => condition.Names.Contains(c.Name));
            }

          if(condition.UserIds != null)
            {
                var userIds = condition.UserIds.Where(c => c > 0).ToList();
                if (userIds.Count > 0)
                    projects = projects.Where(c => condition.UserIds.Contains(c.UserId));
            }

            if (condition.StartedTime != null)
                projects = projects.Where(c => c.StatedTime >= condition.StartedTime.From
                && c.StatedTime <= condition.StartedTime.To);

            if (condition.FinishedTime != null)
                projects = projects.Where(c => c.FinishedTime >= condition.FinishedTime.From
                && c.FinishedTime <= condition.FinishedTime.To);


            var result = new SearchResultViewModel<IList<Project>>();
            result.Total = await projects.CountAsync();

            //sort
            projects = _dbService.Sort(projects, SortDirection.Ascending, ProjectSortProperty.Id);

            //pagination
            projects = _dbService.Paginate(projects, condition.Pagination);

            result.Records = await projects.ToListAsync();
            return Ok(result);
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

            //Check exists project in database
            var isExists = await _dbContext.Projects.AnyAsync(c => c.Name == model.Name);
            if (isExists)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "EXISTS_CODE_ERROR"));

            //Inial Project object
            var project = new Project();
            project.UserId = model.UserId;
            project.Name = model.Name;
            project.Description = model.Description;
            project.FinishedTime = model.FinishedTime;
            project.StatedTime = model.StatedTime;

            //Add project to database
            project = _dbContext.Projects.Add(project);

            //Save changes to database
            await _dbContext.SaveChangesAsync();

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

            //find  Project
            var project = await _dbContext.Projects.FindAsync(id);
            if (project == null)
                return NotFound();
            
            //Update information
            project.UserId = model.UserId;
            project.Name = model.Name;
            project.Description = model.Description;
            project.FinishedTime = model.FinishedTime;
            project.StatedTime = model.StatedTime;

            //Save changes to database
            await _dbContext.SaveChangesAsync();

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
            //Find project in database
            var project = _dbContext.Projects.Find(id);

            if (project == null)
                return NotFound();

            //Delete project from database
            _dbContext.Projects.Remove(project);

            //Save changes to database
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}