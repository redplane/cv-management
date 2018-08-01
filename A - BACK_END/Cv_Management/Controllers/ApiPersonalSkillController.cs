using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Cv_Management.Models.Entities;
using Cv_Management.Models.Entities.Context;
using Cv_Management.ViewModel;
using Cv_Management.ViewModel.PersonalSkill;
using Cv_Management.ViewModel.Skill;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/personalSkill")]
    public class ApiPersonalSkillController : ApiController
    {
        #region Properties

        public readonly CvManagementDbContext DbSet;

        #endregion

        #region Contructors

        public ApiPersonalSkillController()
        {
            DbSet = new CvManagementDbContext();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get Personal skill using specific conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Search([FromBody]SearchPersonalSkillViewModel model)
        {
            model = model ?? new SearchPersonalSkillViewModel();
            var personalSkills = DbSet.PersonalSkills.AsQueryable();


            if (model.SkillCategoryIds != null)
            {
                var skillCategoryIds = model.SkillCategoryIds.Where(x => x > 0).ToList();
                if (skillCategoryIds.Count > 0)
                    personalSkills = personalSkills.Where(x => skillCategoryIds.Contains(x.SkillCategoryId));
            }

            if (model.SkillIds != null)
            {
                var skillIds = model.SkillIds.Where(x => x > 0).ToList();
                if (skillIds.Count > 0)
                    personalSkills = personalSkills.Where(x => skillIds.Contains(x.SkillId));
            }
            if (model.Point > 0)
                personalSkills = personalSkills.Where(c => c.Point == model.Point);
            var result = new SearchResultViewModel<IList<SkillCategorySkillRelationship>>();
            result.Total = await personalSkills.CountAsync();
            var pagination = model.Pagination;
          
            result.Records = await personalSkills.ToListAsync();
            return Ok(result);

        }


        /// <summary>
        /// Create personal skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody]CreatePersonalSkillViewModel model)
        {
            if (model == null)
            {
                model = new CreatePersonalSkillViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var personalSkill = new SkillCategorySkillRelationship();
            personalSkill.SkillCategoryId = model.SkillCategoryId;
            personalSkill.SkillId = model.SkillId;
            personalSkill.Point = model.Point;
            personalSkill.CreatedTime = DateTime.Now.ToOADate();
            personalSkill = DbSet.PersonalSkills.Add(personalSkill);
           await DbSet.SaveChangesAsync();
            return Ok(personalSkill);

        }


        /// <summary>
        /// Update personal skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> Update([FromBody]UpdatePersonalSkillViewModel model)
        {
            if (model == null)
            {
                model = new UpdatePersonalSkillViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var personalSkill = DbSet.PersonalSkills.FirstOrDefault(c => c.SkillCategoryId == model.SkillCategoryId && c.SkillId == model.SkillId);
            if (personalSkill == null)
                return NotFound();
            personalSkill.Point = model.Point;
           await DbSet.SaveChangesAsync();
            return Ok(personalSkill);

        }

        /// <summary>
        /// Delete personal skill
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="skillCategoryId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete([FromUri] int skillId, [FromUri] int skillCategoryId)
        {
            var personalSkill = DbSet.PersonalSkills.FirstOrDefault(c => c.SkillId == skillId && c.SkillCategoryId == skillCategoryId);

            if (personalSkill == null)
                return NotFound();
            DbSet.PersonalSkills.Remove(personalSkill);
            await DbSet.SaveChangesAsync();
            return Ok();

        }

        #endregion

    }
}