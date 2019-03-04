using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using ApiClientShared.Enums;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.PersonalSkill;
using ApiClientShared.ViewModel.SkillCategory;
using ApiClientShared.ViewModel.SkillCategorySkillRelationship;
using CvManagement.Interfaces.Services;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/skill-category-skill")]
    public class ApiSkillCategorySkillController : ApiController
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Database service to handle common db operation.
        /// </summary>
        private readonly IDbService _dbService;

        /// <summary>
        /// Service to handle profile in request.
        /// </summary>
        private readonly IProfileService _profileService;

        #endregion

        #region Contructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="dbService"></param>
        /// <param name="profileService"></param>
        public ApiSkillCategorySkillController(IUnitOfWork unitOfWork,
            IDbService dbService, IProfileService profileService)
        {
            _unitOfWork = unitOfWork;
            _dbService = dbService;
            _profileService = profileService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add has skill relationship into database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddHasSkillRelationship([FromBody] AddHasSkillViewModel model)
        {
            if (model == null)
            {
                model = new AddHasSkillViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            // Find skill category using id.

            var skillCategories = _unitOfWork.SkillCategories.Search();
            skillCategories = skillCategories.Where(x => x.Id == model.SkillCategoryId);
            var skillCategory = await skillCategories.FirstOrDefaultAsync();
            if (skillCategory == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    HttpMessages.SkillCategoryNotFound));

            // Validate the skills list.
            IDictionary<int, int> mHasSkills = null;
            IList<int> skillIds = null;

            if (model.HasSkills != null)
            {
                mHasSkills = model.HasSkills.ToDictionary(key => key.SkillId, value => value.Point);
                skillIds = mHasSkills.Keys.ToList();

                if (skillIds.Count > 0)
                {
                    var skills = _unitOfWork.Skills.Search();
                    skills = skills.Where(skill => skillIds.Contains(skill.Id));

                    var iTotalValidSkills = await skills.CountAsync();
                    if (iTotalValidSkills != mHasSkills.Count)
                    {
                        ModelState.AddModelError($"{nameof(model)}.{nameof(model.HasSkills)}", HttpMessages.SkillInvalid);
                        return BadRequest(ModelState);
                    }
                }
            }
            
            // Begin a transaction.
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Remove all skill category - skill relationships
                var hasSkills = _unitOfWork.SkillCategorySkillRelationships.Search();
                hasSkills = hasSkills.Where(x => x.SkillCategoryId == skillCategory.Id);
                _unitOfWork.SkillCategorySkillRelationships.Remove(hasSkills);

                // Go through every skill ids and add 'em to the list.
                if (model.HasSkills != null)
                {
                    foreach (var mHasSkill in model.HasSkills)
                    {
                        var hasSkill = new SkillCategorySkillRelationship();
                        hasSkill.SkillCategoryId = skillCategory.Id;
                        hasSkill.SkillId = mHasSkill.SkillId;
                        hasSkill.Point = mHasSkill.Point;
                        hasSkill.CreatedTime = 0;
                        _unitOfWork.SkillCategorySkillRelationships.Insert(hasSkill);
                    }
                }

                // Save changes into database.
                await _unitOfWork.CommitAsync();
                transactionScope.Complete();
            }

            return Ok();
        }

        /// <summary>
        /// Edit has skill relationship.
        /// </summary>
        /// <param name="skillCategoryId"></param>
        /// <param name="skillId"></param>
        /// <param name="editHasSkill"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<IHttpActionResult> EditHasSkillRelationship([FromUri] int skillCategoryId,
            [FromUri] int skillId, [FromBody] EditHasSkillViewModel editHasSkill)
        {
            var hasSkills = _unitOfWork.SkillCategorySkillRelationships.Search();
            hasSkills = hasSkills.Where(x => x.SkillCategoryId == skillCategoryId && x.SkillId == skillId);

            var hasSkill = await hasSkills.FirstOrDefaultAsync();
            if (hasSkill == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    HttpMessages.HasSkillNotFound));

            hasSkill.Point = editHasSkill.Point;
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        /// <summary>
        ///     Get Personal skill using specific conditions
        /// </summary>
        /// <param name="conditon"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
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

            // Get request profile.
            var profile = _profileService.GetProfile(Request);

            //Get personal skill
            var skillCategorySkillRelationships = _unitOfWork.SkillCategorySkillRelationships.Search();

            //Search for skillCategoryIds
            if (conditon.SkillCategoryIds != null && conditon.SkillCategoryIds.Any())
            {
                var skillCategoryIds = conditon.SkillCategoryIds.Where(x => x > 0).ToList();
                if (skillCategoryIds.Count > 0)
                    skillCategorySkillRelationships = skillCategorySkillRelationships.Where(x => skillCategoryIds.Contains(x.SkillCategoryId));
            }

            //Search for skillIds
            if (conditon.SkillIds != null && conditon.SkillIds.Any())
            {
                var skillIds = conditon.SkillIds.Where(x => x > 0).ToList();
                if (skillIds.Count > 0)
                    skillCategorySkillRelationships = skillCategorySkillRelationships.Where(x => skillIds.Contains(x.SkillId));
            }

            //Search for point
            var point = conditon.Point;
            if (conditon.Point != null)
            {
                if (point.From != null)
                    skillCategorySkillRelationships = skillCategorySkillRelationships.Where(c => c.Point >= point.From);

                if (point.To != null)
                    skillCategorySkillRelationships = skillCategorySkillRelationships.Where(x => x.Point <= point.To);
            }

            // Find the list of users.
            var users = _unitOfWork.Users.Search();
            if (conditon.UserIds != null && conditon.UserIds.Count > 0)
            {
                var userIds = conditon.UserIds.Where(x => x > 0).ToList();
                users = users.Where(x => userIds.Contains(x.Id));
            }

            // Ordinary user can only search for skill category - skill relationship of other active users.
            if (profile == null || profile.Role != UserRoles.Admin)
            {
                users = users.Where(x => x.Status == UserStatuses.Active);

                // Find the list of skill category belongs to those user.
                var skillCategories = _unitOfWork.SkillCategories.Search();

                var relationships = skillCategorySkillRelationships;
                skillCategorySkillRelationships = from user in users
                                                  from skillCategory in skillCategories
                                                  from skillCategorySkillRelationship in relationships
                                                  where user.Id == skillCategory.UserId &&
                                                        skillCategory.Id == skillCategorySkillRelationship.SkillCategoryId
                                                  select skillCategorySkillRelationship;
            }

            //Result
            var result = new SearchResultViewModel<IList<SkillCategorySkillRelationship>>();
            result.Total = await skillCategorySkillRelationships.CountAsync();

            //Do Sort
            //  personalSkills = 
            result.Records = await skillCategorySkillRelationships.ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Delete relationships using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> DeleteRelationship(
            [FromUri] DeleteSkillCategorySkillRelationshipViewModel condition)
        {
            if (condition == null)
            {
                condition = new DeleteSkillCategorySkillRelationshipViewModel();
                Validate(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var skillCategorySkillRelationships = _unitOfWork.SkillCategorySkillRelationships.Search();
            skillCategorySkillRelationships = skillCategorySkillRelationships.Where(x =>
                x.SkillCategoryId == condition.SkillCategoryId && x.SkillId == condition.SkillId);

            _unitOfWork.SkillCategorySkillRelationships.Remove(skillCategorySkillRelationships);
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        #endregion
    }
}