using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectSkill;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/project-skill")]
    public class ApiProjectSkillController : ApiController
    {
        #region Properties

        public readonly CvManagementDbContext _dbContext;

        public readonly IDbService _dbService;

        #endregion

        #region Contructors

        public ApiProjectSkillController(DbContext dbContext,
            IDbService dbService)
        {
            _dbContext =  (CvManagementDbContext)dbContext;
            _dbService = dbService;
        }

        #endregion

        #region Mothods

        /// <summary>
        ///     get projects skill using specific conditions
        /// </summary>
        /// <param name="model"></param>
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
                return BadRequest();

            //Get project skills list
            var projectSkills = _dbContext.ProjectSkills.AsQueryable();
            if (condition.ProjectIds != null)
            {
                var projectIds = condition.ProjectIds.Where(x => x > 0).ToList();
                if (projectIds.Count > 0)
                    projectSkills = projectSkills.Where(x => projectIds.Contains(x.ProjectId));
            }
            if (condition.SkillIds != null)
            {
                var skillIds = condition.SkillIds.Where(x => x > 0).ToList();
                if (skillIds.Count > 0)
                    projectSkills = projectSkills.Where(x => skillIds.Contains(x.SkillId));
            }

            //Result search 
            var result = new SearchResultViewModel<IList<ProjectSkill>>();
            result.Total = await projectSkills.CountAsync();
            
            //Do sort
            projectSkills = _dbService.Sort(projectSkills, SortDirection.Ascending, ProjectSkillSortProperty.Id);

            //Do pagination
            projectSkills = _dbService.Paginate(projectSkills, condition.Pagination);

            result.Records = await projectSkills.ToListAsync();
            return Ok(result);
        }

        ///// <summary>
        /////     Create project skill
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("")]
        //public async Task<IHttpActionResult> Create([FromBody] AddProjectSkillViewModel model)
        //{
        //    if (model == null)
        //    {
        //        model = new AddProjectSkillViewModel();
        //        Validate(model);
        //    }
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    var projectSkill = new ProjectSkill();
        //    projectSkill.ProjectId = model.ProjectId;
        //    projectSkill.SkillId = model.SkillId;
        //    projectSkill = DbSet.ProjectSkills.Add(projectSkill);
        //    await DbSet.SaveChangesAsync();
        //    return Ok(projectSkill);
        //}

        ///// <summary>
        /////     Delete Project skill
        ///// </summary>
        ///// <param name="skillId"></param>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //[HttpDelete]
        //[Route("")]
        //public async Task<IHttpActionResult> Delete([FromUri] int skillId, [FromUri] int projectId)
        //{
        //    var projectSkill =
        //        DbSet.ProjectSkills.FirstOrDefault(c => c.SkillId == skillId && c.ProjectId == projectId);

        //    if (projectSkill == null)
        //        return NotFound();
        //    DbSet.ProjectSkills.Remove(projectSkill);
        //    await DbSet.SaveChangesAsync();
        //    return Ok();
        //}

        #endregion
    }
}