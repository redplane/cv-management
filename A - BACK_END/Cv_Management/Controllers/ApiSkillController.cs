using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Skill;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/skill")]
    public class ApiSkillController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        /// <param name="profileService"></param>
        public ApiSkillController(DbContext dbContext, IDbService dbService, IProfileService profileService)
        {
            _dbContext = (CvManagementDbContext) dbContext;
            _dbService = dbService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Context to access to database
        /// </summary>
        private readonly CvManagementDbContext _dbContext;


        /// <summary>
        ///     Service to handle database operation
        /// </summary>
        private readonly IDbService _dbService;

        #endregion

        #region Methods

        /// <summary>
        ///     Get skills using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchSkillViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchSkillViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var skills = _dbContext.Skills.AsQueryable();

            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                var ids = condition.Ids.Where(c => c > 0).ToList();
                if (ids.Count > 0)
                    skills = skills.Where(c => ids.Contains(c.Id));
            }

            if (condition.Names != null && condition.Names.Count > 0)
            {
                var names = condition.Names.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (names.Count > 0)
                    skills = skills.Where(c => names.Any(name => c.Name.Contains(name)));
            }

            if (condition.StartedTime != null)
                skills = skills.Where(c => c.CreatedTime >= condition.StartedTime.From
                                           && c.CreatedTime <= condition.StartedTime.To);

            var result = new SearchResultViewModel<IList<Skill>>();
            result.Total = await skills.CountAsync();

            //do sorting
            skills = _dbService.Sort(skills, SortDirection.Ascending, UserDescriptionSortProperty.Id);

            // Do pagination.
            skills = _dbService.Paginate(skills, condition.Pagination);

            result.Records = await skills.ToListAsync();

            return Ok(result);
        }

        /// <summary>
        ///     Create skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddSkill(AddSkillViewModel model)
        {
            if (model == null)
            {
                model = new AddSkillViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check exists skill in database
            var isExists = await _dbContext.Skills.AnyAsync(c => c.Name.Equals(model.Name));
            if (isExists)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict,
                    "EXISTS_CODE_ERROR"));

            //Inital skill object
            var skill = new Skill();
            skill.Name = model.Name;
            skill.CreatedTime = DateTime.Now.ToOADate();

            //add skill to database
            skill = _dbContext.Skills.Add(skill);

            //save changes to database
            await _dbContext.SaveChangesAsync();

            return Ok(skill);
        }


        /// <summary>
        ///     Update skill
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> EditSkill([FromUri] int id, [FromBody] EditSkillViewModel model)
        {
            if (model == null)
            {
                model = new EditSkillViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Find skill in database
            var skill = await _dbContext.Skills.FindAsync(id);
            if (skill == null)
                return NotFound();

            //Update information
            skill.Name = model.Name;
            skill.LastModifiedTime = DateTime.Now.ToOADate();

            //Save changes to database
            await _dbContext.SaveChangesAsync();

            return Ok(skill);
        }

        /// <summary>
        ///     Delete skill from id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteSkill([FromUri] int id)
        {
            //Find skill in database
            var skill = await _dbContext.Skills.FindAsync(id);
            if (skill == null)
                return NotFound();

            //Delete skill from database
            _dbContext.Skills.Remove(skill);

            //Save changes in database
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}