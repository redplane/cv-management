using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Cv_Management.Entities;
using Cv_Management.Entities.Context;
using Cv_Management.ViewModel;
using Cv_Management.ViewModel.Project;
using Cv_Management.ViewModel.ProjectResponsibility;
using Cv_Management.ViewModel.Responsibility;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/project-responsibility")]
    public class ApiProjectResponsibilityController : ApiController
    {
        #region properties

        public readonly CvManagementDbContext DbSet;

        #endregion

        #region Contructors

        public ApiProjectResponsibilityController()
        {
            DbSet = new CvManagementDbContext();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get project responsibility using specific conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Search([FromBody]SearchProjectResponsibilityViewModel model)
        {
            model = model ?? new SearchProjectResponsibilityViewModel();
            var projectResponsibilities = DbSet.ProjectResponsibilities.AsQueryable();
            if (model.ProjectIds != null)
            {
                var projectIds = model.ProjectIds.Where(x => x > 0).ToList();
                if (projectIds.Count > 0)
                    projectResponsibilities = projectResponsibilities.Where(x => projectIds.Contains(x.ProjectId));

            }
            if (model.ResponsibilityIds != null)
            {
                var responsibilityIds = model.ResponsibilityIds.Where(x => x > 0).ToList();
                if (responsibilityIds.Count > 0)
                    projectResponsibilities = projectResponsibilities.Where(x => responsibilityIds.Contains(x.RespinsibilityId));

            }

            var result = new SearchResultViewModel<IList<ProjectResponsibility>>();
            result.Total = await projectResponsibilities.CountAsync();

            var pagination = model.Pagination;
            if (pagination != null)
            {
                if (pagination.Page < 1)
                    pagination.Page = 1;

                projectResponsibilities = projectResponsibilities.Skip((pagination.Page - 1) * pagination.Records)
                    .Take(pagination.Records);
            }
            result.Records = await projectResponsibilities.ToListAsync();
            return Ok(result);

        }

        /// <summary>
        /// Create project responsibility
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody]CreateProjectResponsibilityViewModel model)
        {
            if (model == null)
            {
                model = new CreateProjectResponsibilityViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var projectResponsibility = new ProjectResponsibility();
            projectResponsibility.ProjectId = model.ProjectId;
            projectResponsibility.RespinsibilityId = model.ResponsibilityId;
            projectResponsibility.CreatedTime = DateTime.Now.ToOADate();
            projectResponsibility = DbSet.ProjectResponsibilities.Add(projectResponsibility);
            await DbSet.SaveChangesAsync();
            return Ok(projectResponsibility);

        }

        /// <summary>
        /// Delete project responsibility
        /// </summary>
        /// <param name="responsibilityId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete([FromUri] int responsibilityId, [FromUri] int projectId)
        {
            var projectResponsibility = DbSet.ProjectResponsibilities.FirstOrDefault(c => c.RespinsibilityId == responsibilityId && c.ProjectId == projectId);

            if (projectResponsibility == null)
                return NotFound();
            DbSet.ProjectResponsibilities.Remove(projectResponsibility);
            await DbSet.SaveChangesAsync();
            return Ok();

        }

        #endregion

    }
}