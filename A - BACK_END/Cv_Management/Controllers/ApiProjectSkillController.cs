using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectSkill;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/projectSkill")]
    public class ApiProjectSkillController : ApiController
    {
        #region Properties

        public readonly CvManagementDbContext DbSet;

        #endregion

        #region Contructors

        public ApiProjectSkillController()
        {
            DbSet = new CvManagementDbContext();
        }

        #endregion

        #region Mothods

        /// <summary>
        ///     get projects skill using specific conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectSkillViewModel model)
        {
            model = model ?? new SearchProjectSkillViewModel();
            var projectSkills = DbSet.ProjectSkills.AsQueryable();
            if (model.ProjectIds != null)
            {
                var projectIds = model.ProjectIds.Where(x => x > 0).ToList();
                if (projectIds.Count > 0)
                    projectSkills = projectSkills.Where(x => projectIds.Contains(x.ProjectId));
            }
            if (model.SkillIds != null)
            {
                var skillIds = model.SkillIds.Where(x => x > 0).ToList();
                if (skillIds.Count > 0)
                    projectSkills = projectSkills.Where(x => skillIds.Contains(x.SkillId));
            }
            var result = new SearchResultViewModel<IList<ProjectSkill>>();
            result.Total = await projectSkills.CountAsync();
            var pagination = model.Pagination;

            result.Records = await projectSkills.ToListAsync();
            return Ok(result);
        }

        /// <summary>
        ///     Create project skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody] AddProjectSkillViewModel model)
        {
            if (model == null)
            {
                model = new AddProjectSkillViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var projectSkill = new ProjectSkill();
            projectSkill.ProjectId = model.ProjectId;
            projectSkill.SkillId = model.SkillId;
            projectSkill = DbSet.ProjectSkills.Add(projectSkill);
            await DbSet.SaveChangesAsync();
            return Ok(projectSkill);
        }

        /// <summary>
        ///     Delete Project skill
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete([FromUri] int skillId, [FromUri] int projectId)
        {
            var projectSkill =
                DbSet.ProjectSkills.FirstOrDefault(c => c.SkillId == skillId && c.ProjectId == projectId);

            if (projectSkill == null)
                return NotFound();
            DbSet.ProjectSkills.Remove(projectSkill);
            await DbSet.SaveChangesAsync();
            return Ok();
        }

        #endregion
    }
}