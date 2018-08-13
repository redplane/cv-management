using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectResponsibility;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/project-responsibility")]
    public class ApiProjectResponsibilityController : ApiController
    {
        #region properties

        public readonly CvManagementDbContext _dbContext;

        public readonly IDbService _dbService;
        #endregion

        #region Contructors

        public ApiProjectResponsibilityController(DbContext dbContext,
            IDbService dbService)
        {
            _dbContext = (CvManagementDbContext)dbContext;
            _dbService = dbService;

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
                return BadRequest();

            //Get list projet responsibility
            var projectResponsibilities = _dbContext.ProjectResponsibilities.AsQueryable();
            if (condition.ProjectIds != null)
            {
                var projectIds = condition.ProjectIds.Where(x => x > 0).ToList();
                if (projectIds.Count > 0)
                    projectResponsibilities = projectResponsibilities.Where(x => projectIds.Contains(x.ProjectId));
            }
            if (condition.ResponsibilityIds != null)
            {
                var responsibilityIds = condition.ResponsibilityIds.Where(x => x > 0).ToList();
                if (responsibilityIds.Count > 0)
                    projectResponsibilities =
                        projectResponsibilities.Where(x => responsibilityIds.Contains(x.ResponsibilityId));
            }

            var result = new SearchResultViewModel<IList<ProjectResponsibility>>();
            result.Total = await projectResponsibilities.CountAsync();

            //Do sort
            projectResponsibilities =
                _dbService.Sort(projectResponsibilities, SortDirection.Ascending, ProjectSortProperty.Id);

            //Do paginatin
            projectResponsibilities = _dbService.Paginate(projectResponsibilities, condition.Pagination);

            result.Records = await projectResponsibilities.ToListAsync();

            return Ok(result);
        }

        /// <summary>
        ///     Create project responsibility
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("")]
        //public async Task<IHttpActionResult> Create([FromBody] AddProjectResponsibilityViewModel model)
        //{
        //    if (model == null)
        //    {
        //        model = new AddProjectResponsibilityViewModel();
        //        Validate(model);
        //    }
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    var projectResponsibility = new ProjectResponsibility();
        //    projectResponsibility.ProjectId = model.ProjectId;
        //    projectResponsibility.ResponsibilityId = model.ResponsibilityId;
        //    projectResponsibility.CreatedTime = DateTime.Now.ToOADate();
        //    projectResponsibility = DbSet.ProjectResponsibilities.Add(projectResponsibility);
        //    await DbSet.SaveChangesAsync();
        //    return Ok(projectResponsibility);
        //}

        ///// <summary>
        /////     Delete project responsibility
        ///// </summary>
        ///// <param name="responsibilityId"></param>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //[HttpDelete]
        //[Route("")]
        //public async Task<IHttpActionResult> Delete([FromUri] int responsibilityId, [FromUri] int projectId)
        //{
        //    var projectResponsibility = DbSet.ProjectResponsibilities.FirstOrDefault(c =>
        //        c.ResponsibilityId == responsibilityId && c.ProjectId == projectId);

        //    if (projectResponsibility == null)
        //        return NotFound();
        //    DbSet.ProjectResponsibilities.Remove(projectResponsibility);
        //    await DbSet.SaveChangesAsync();
        //    return Ok();
        //}

        #endregion
    }
}