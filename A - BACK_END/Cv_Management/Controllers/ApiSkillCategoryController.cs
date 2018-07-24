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
using Cv_Management.ViewModel.SkillCategory;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/skillCategory")]
    public class ApiSkillCategoryController : ApiController
    {
        #region Properties

        public readonly CvManagementDbContext DbSet;

        #endregion


        #region Contructors

        public ApiSkillCategoryController()
        {
            DbSet = new CvManagementDbContext();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Get Skill category using specific conditions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Search([FromBody]SearchSkillCategoryViewModel model)
        {
            model = model ?? new SearchSkillCategoryViewModel();
            var skillCategories = DbSet.SkillCategories.AsQueryable();
            if (model.Ids != null)
            {
                var ids = model.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    skillCategories = skillCategories.Where(x => ids.Contains(x.Id));

            }

            if (model.UserId > 0)
                skillCategories = skillCategories.Where(c => c.UserId == model.UserId);
            if (!string.IsNullOrEmpty(model.Name))
                skillCategories = skillCategories.Where(c => c.Name.Contains(model.Name));
            var result = new SearchResultViewModel<IList<SkillCategory>>();
            result.Total = await skillCategories.CountAsync();
            var pagination = model.Pagination;
            if (pagination != null)
            {
                if (pagination.Page < 1)
                    pagination.Page = 1;
                skillCategories = skillCategories.Skip((pagination.Page - 1) * pagination.Records)
                    .Take(pagination.Records);
            }
            result.Records = await skillCategories.ToListAsync();
            return Ok(result);

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
            skillCategory = DbSet.SkillCategories.Add(skillCategory);
            await DbSet.SaveChangesAsync();
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
            var skillCategory = DbSet.SkillCategories.Find(id);
            if (skillCategory == null)
                return NotFound();
            skillCategory.Name = model.Name;
            skillCategory.UserId = model.UserId;
            if (model.Photo != null)
                skillCategory.Photo = Convert.ToBase64String(model.Photo.Buffer);
            await DbSet.SaveChangesAsync();
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
            var skillCategory = DbSet.SkillCategories.Find(id);
            if (skillCategory == null)
                return NotFound();
            DbSet.SkillCategories.Remove(skillCategory);
            await DbSet.SaveChangesAsync();
            return Ok();

        }

        #endregion

    }
}