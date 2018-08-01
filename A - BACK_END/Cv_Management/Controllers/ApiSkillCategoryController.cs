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
using Cv_Management.ViewModel.SkillCategory;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/skill-category")]
    public class ApiSkillCategoryController : ApiController
    {
        #region Properties

        private readonly CvManagementDbContext _dbContext;

        #endregion

        #region Contructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="dbContext"></param>
        public ApiSkillCategoryController(DbContext dbContext)
        {
            _dbContext = dbContext as CvManagementDbContext;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Get Skill category using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody]SearchSkillCategoryViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchSkillCategoryViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Information search

            // Get list of skill categories.
            var skillCategories = _dbContext.SkillCategories.AsQueryable();

            // Filter skill categories by indexes.
            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                var ids = condition.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    skillCategories = skillCategories.Where(x => ids.Contains(x.Id));
            }

            // Filter skill categories by user indexes.
            if (condition.UserIds != null && condition.UserIds.Count > 0)
            {
                var userIds = condition.UserIds.Where(x => x > 0).ToList();
                if (userIds.Count > 0)
                    skillCategories = skillCategories.Where(x => userIds.Contains(x.UserId));
            }

            // Filter skill categories by user indexes.
            if (condition.Names != null && condition.Names.Count > 0)
            {
                var names = condition.Names.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                if (names.Count > 0)
                    skillCategories = skillCategories.Where(x => names.Any(name => x.Name.Contains(name)));
            }

            #endregion
            
            // Get offline skill categories.
            var loadSkillCategoryResult = new SearchResultViewModel<IList<SkillCategory>>();
            //loadSkillCategoryResult.Total = await skillCategories.CountAsync();
            //loadSkillCategoryResult.Records = await skillCategories.ToListAsync();
            return Ok(loadSkillCategoryResult);

        }

        /// <summary>
        /// Create Skill category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody]CreateSkillCategoryViewModel model)
        {
            if (model == null)
            {
                model = new CreateSkillCategoryViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var skillCategory = new SkillCategory();
            skillCategory.Name = model.Name;
            skillCategory.UserId = model.UserId;
            if (model.Photo != null)
                skillCategory.Photo = Convert.ToBase64String(model.Photo.Buffer);
            skillCategory.CreatedTime = DateTime.Now.ToOADate();
            skillCategory = _dbContext.SkillCategories.Add(skillCategory);
            await _dbContext.SaveChangesAsync();
            return Ok(skillCategory);

        }

        /// <summary>
        /// Update skill category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody]UpdateSkillCategoryViewModel model)
        {
            if (model == null)
            {
                model = new UpdateSkillCategoryViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //get SkillCategory
            var skillCategory = _dbContext.SkillCategories.Find(id);
            if (skillCategory == null)
                return NotFound();
            skillCategory.Name = model.Name;
            skillCategory.UserId = model.UserId;
            if (model.Photo != null)
                skillCategory.Photo = Convert.ToBase64String(model.Photo.Buffer);
            await _dbContext.SaveChangesAsync();
            return Ok(skillCategory);

        }

        /// <summary>
        /// Delete skill category with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete([FromUri]int id)
        {
            var skillCategory = _dbContext.SkillCategories.Find(id);
            if (skillCategory == null)
                return NotFound();
            _dbContext.SkillCategories.Remove(skillCategory);
            await _dbContext.SaveChangesAsync();
            return Ok();

        }

        #endregion

    }
}