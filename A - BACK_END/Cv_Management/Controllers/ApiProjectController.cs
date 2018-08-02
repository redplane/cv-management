using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Cv_Management.ViewModel;
using Cv_Management.ViewModel.Project;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/projet")]
    public class ApiProjectController : ApiController
    {
        #region Properties

        public readonly CvManagementDbContext DbSet;

        #endregion

        #region Contructors

        public ApiProjectController()
        {
            DbSet = new CvManagementDbContext();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get projects using specific conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectViewModel model)
        {
            model = model ?? new SearchProjectViewModel();
            var projects = DbSet.Projects.AsQueryable();
            if (model.Ids != null)
            {
                var ids = model.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    projects = projects.Where(x => ids.Contains(x.Id));
            }
            if (!string.IsNullOrEmpty(model.Name))
                projects = projects.Where(c => c.Name.Contains(model.Name));
            var result = new SearchResultViewModel<IList<Project>>();
            result.Total = await projects.CountAsync();
            var pagination = model.Pagination;

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
        public async Task<IHttpActionResult> Create([FromBody] CreateProjectViewModel model)
        {
            if (model == null)
            {
                model = new CreateProjectViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var project = new Project();
            project.UserId = model.UserId;
            project.Name = model.Name;
            project.Description = model.Description;
            project.FinishedTime = model.FinishedTime;
            project.StatedTime = model.StatedTime;
            project = DbSet.Projects.Add(project);
            await DbSet.SaveChangesAsync();
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
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody] UpdateProjectViewModel model)
        {
            if (model == null)
            {
                model = new UpdateProjectViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //get Project
            var project = DbSet.Projects.Find(id);
            if (project == null)
                return NotFound();
            project.UserId = model.UserId;
            project.Name = model.Name;
            project.Description = model.Description;
            project.FinishedTime = model.FinishedTime;
            project.StatedTime = model.StatedTime;
            await DbSet.SaveChangesAsync();
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
            var project = DbSet.Projects.Find(id);
            if (project == null)
                return NotFound();
            DbSet.Projects.Remove(project);
            await DbSet.SaveChangesAsync();
            return Ok();
        }

        #endregion
    }
}