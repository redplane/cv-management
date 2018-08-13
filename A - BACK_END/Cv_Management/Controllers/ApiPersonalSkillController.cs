using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.PersonalSkill;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/personal-skill")]
    public class ApiPersonalSkillController : ApiController
    {
        #region Properties

        public readonly CvManagementDbContext _dbContext;

        public readonly IDbService _dbService;

        #endregion

        #region Contructors

        public ApiPersonalSkillController(DbContext dbContext,
            IDbService dbService)
        {
            _dbContext = (CvManagementDbContext)dbContext;
            _dbService = dbService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get Personal skill using specific conditions
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchPersonalSkillViewModel conditon)
        {
           //Check null for model
            if (conditon == null)
            {
                conditon = new SearchPersonalSkillViewModel();
                Validate(conditon);
            }

            //Validate for model
            if (!ModelState.IsValid)
                return BadRequest();

            //Get personal skill
            var personalSkills = _dbContext.PersonalSkills.AsQueryable();

            //Search for skillCategoryIds
            if (conditon.SkillCategoryIds != null && conditon.SkillCategoryIds.Any())
            {
                var skillCategoryIds = conditon.SkillCategoryIds.Where(x => x > 0).ToList();
                if (skillCategoryIds.Count > 0)
                    personalSkills = personalSkills.Where(x => skillCategoryIds.Contains(x.SkillCategoryId));
            }

            //Search for skillIds
            if (conditon.SkillIds != null && conditon.SkillIds.Any())
            {
                var skillIds = conditon.SkillIds.Where(x => x > 0).ToList();
                if (skillIds.Count > 0)
                    personalSkills = personalSkills.Where(x => skillIds.Contains(x.SkillId));
            }

            //Search for point
            if (conditon.Point > 0)
                personalSkills = personalSkills.Where(c => c.Point == conditon.Point);

            //Result
            var result = new SearchResultViewModel<IList<SkillCategorySkillRelationship>>();
            result.Total = await personalSkills.CountAsync();
           
            //Do Sort
          //  personalSkills = 
            result.Records = await personalSkills.ToListAsync();
            return Ok(result);
        }


        /// <summary>
        ///     Create personal skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("")]
        //public async Task<IHttpActionResult> Create([FromBody] AddPersonalSkillViewModel model)
        //{
        //    if (model == null)
        //    {
        //        model = new AddPersonalSkillViewModel();
        //        Validate(model);
        //    }
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    var personalSkill = new SkillCategorySkillRelationship();
        //    personalSkill.SkillCategoryId = model.SkillCategoryId;
        //    personalSkill.SkillId = model.SkillId;
        //    personalSkill.Point = model.Point;
        //    personalSkill.CreatedTime = DateTime.Now.ToOADate();
        //    personalSkill = DbSet.PersonalSkills.Add(personalSkill);
        //    await DbSet.SaveChangesAsync();
        //    return Ok(personalSkill);
        //}


        ///// <summary>
        /////     Update personal skill
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("")]
        //public async Task<IHttpActionResult> Update([FromBody] EditPersonalSkillViewModel model)
        //{
        //    if (model == null)
        //    {
        //        model = new EditPersonalSkillViewModel();
        //        Validate(model);
        //    }
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    var personalSkill = DbSet.PersonalSkills.FirstOrDefault(c =>
        //        c.SkillCategoryId == model.SkillCategoryId && c.SkillId == model.SkillId);
        //    if (personalSkill == null)
        //        return NotFound();
        //    personalSkill.Point = model.Point;
        //    await DbSet.SaveChangesAsync();
        //    return Ok(personalSkill);
        //}

        ///// <summary>
        /////     Delete personal skill
        ///// </summary>
        ///// <param name="skillId"></param>
        ///// <param name="skillCategoryId"></param>
        ///// <returns></returns>
        //[HttpDelete]
        //[Route("")]
        //public async Task<IHttpActionResult> Delete([FromUri] int skillId, [FromUri] int skillCategoryId)
        //{
        //    var personalSkill =
        //        DbSet.PersonalSkills.FirstOrDefault(c => c.SkillId == skillId && c.SkillCategoryId == skillCategoryId);

        //    if (personalSkill == null)
        //        return NotFound();
        //    DbSet.PersonalSkills.Remove(personalSkill);
        //    await DbSet.SaveChangesAsync();
        //    return Ok();
        //}

        #endregion
    }
}