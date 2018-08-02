using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using Cv_Management.Enums.SortProperties;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models.Entities;
using Cv_Management.Models.Entities.Context;
using Cv_Management.ViewModel;
using Cv_Management.ViewModel.Skill;
using Cv_Management.ViewModel.SkillCategory;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/skill-category")]
    public class ApiSkillCategoryController : ApiController
    {
        #region Properties

        /// <summary>
        /// Database context.
        /// </summary>
        private readonly CvManagementDbContext _dbContext;

        /// <summary>
        /// Service which is for handling database operation.
        /// </summary>
        private readonly IDbService _dbService;

        #endregion

        #region Contructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        public ApiSkillCategoryController(DbContext dbContext, IDbService dbService)
        {
            _dbContext = dbContext as CvManagementDbContext;
            _dbService = dbService;
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
        public async Task<IHttpActionResult> Search([FromBody] SearchSkillCategoryViewModel condition)
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

            // Import skill list.
            var skills = Enumerable.Empty<Skill>().AsQueryable();
            var skillCategorySkillRelationships = Enumerable.Empty<SkillCategorySkillRelationship>().AsQueryable();

            if (condition.IncludePersonalSkills)
                skills = _dbContext.Skills.AsQueryable();
            
            // Get offline skill categories.
            var loadSkillCategoryResult = new SearchResultViewModel<IList<SkillCategoryViewModel>>();
            loadSkillCategoryResult.Total = await skillCategories.CountAsync();

            var loadedSkillCategories = from skillCategory in skillCategories
                select new SkillCategoryViewModel
                {
                    Id = skillCategory.Id,
                    UserId = skillCategory.UserId,
                    Photo = skillCategory.Photo,
                    Name = skillCategory.Name,
                    CreatedTime = skillCategory.CreatedTime,
                    PersonalSkills = from skill in skills
                        from skillCategorySkillRelationship in skillCategorySkillRelationships
                        where skillCategorySkillRelationship.SkillCategoryId == skillCategory.Id &&
                              skillCategorySkillRelationship.SkillId == skill.Id
                        select new PersonalSkillViewModel
                        {
                            SkillCategoryId = skillCategorySkillRelationship.SkillCategoryId,
                            SkillId = skillCategorySkillRelationship.SkillId,
                            Point = skillCategorySkillRelationship.Point,
                            SkillName = skill.Name
                        }
                };

            // Do sorting.
            loadedSkillCategories = _dbService.Sort(loadedSkillCategories, SortDirection.Ascending,
                SkillCategorySortProperty.Id);

            // Do paging.
            loadedSkillCategories = _dbService.Paginate(loadedSkillCategories, condition.Pagination);

            // Get the records list.
            loadSkillCategoryResult.Records = await loadedSkillCategories.ToListAsync();
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