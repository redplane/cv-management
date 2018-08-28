using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Skill;
using ApiClientShared.ViewModel.SkillCategory;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cv_Management.Interfaces.Services;
using Cv_Management.ViewModels.SkillCategory;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;
using System.Drawing.Imaging;
using System.Threading;
using ApiClientShared.Constants;
using ApiClientShared.Resources;
using Cv_Management.Models;
using Image = System.Drawing.Image;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/skill-category")]
    public class ApiSkillCategoryController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        /// <param name="mapper"></param>
        /// <param name="fileService"></param>
        /// <param name="appPath"></param>
        public ApiSkillCategoryController(DbContext dbContext, IDbService dbService, IMapper mapper, IFileService fileService, AppPathModel appPath)
        {
            _dbContext = dbContext as CvManagementDbContext;
            _dbService = dbService;
            _mapper = mapper;
            _fileService = fileService;
            _appPath = appPath;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Database context.
        /// </summary>
        private readonly CvManagementDbContext _dbContext;

        /// <summary>
        ///     Service which is for handling database operation.
        /// </summary>
        private readonly IDbService _dbService;

        /// <summary>
        /// Automapper DI.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Service to handle files.
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// App path option
        /// </summary>
        private readonly AppPathModel _appPath;

        #endregion

        #region Methods

        /// <summary>
        ///     Get Skill category using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
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

            if (condition.IncludeSkills)
            {
                skillCategorySkillRelationships = _dbContext.SkillCategorySkillRelationships.AsQueryable();
                skills = _dbContext.Skills.AsQueryable();
            }

            // Get offline skill categories.
            var loadSkillCategoryResult = new SearchResultViewModel<IEnumerable<SkillCategoryViewModel>>();
            loadSkillCategoryResult.Total = await skillCategories.CountAsync();
            
            // Load skill categories.
            var loadedSkillCategories = from skillCategory in skillCategories
                                        select new SkillCategoryViewModel
                                        {
                                            Id = skillCategory.Id,
                                            UserId = skillCategory.UserId,
                                            Photo = skillCategory.Photo,
                                            Name = skillCategory.Name,
                                            CreatedTime = skillCategory.CreatedTime,
                                            Skills = from skill in skills
                                                     from skillCategorySkillRelationship in skillCategorySkillRelationships
                                                     where skillCategorySkillRelationship.SkillCategoryId == skillCategory.Id &&
                                                           skillCategorySkillRelationship.SkillId == skill.Id
                                                     select new SkillCategorySkillRelationshipViewModel
                                                     {
                                                         UserId = skillCategory.UserId,
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
            loadSkillCategoryResult.Records = await loadedSkillCategories.ToListAsync();

            return Ok(loadSkillCategoryResult);
        }

        /// <summary>
        ///     Create Skill category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddSkillCategory([FromBody] AddSkillCategoryViewModel model)
        {
            //Check null for model
            if (model == null)
            {
                model = new AddSkillCategoryViewModel();
                Validate(model);
            }

            // Validate for model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var skillCategory = new SkillCategory();
            skillCategory.Name = model.Name;
            skillCategory.UserId = model.UserId;
            if (model.Photo != null)
                skillCategory.Photo = Convert.ToBase64String(model.Photo.Buffer);
            skillCategory.CreatedTime = DateTime.Now.ToOADate();

            //Save to db context
            skillCategory = _dbContext.SkillCategories.Add(skillCategory);

            //save change to db
            await _dbContext.SaveChangesAsync();

            return Ok(skillCategory);
        }

        /// <summary>
        ///     Update skill category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> EditSkillCategory([FromUri] int id, [FromBody] EditSkillCategoryViewModel model)
        {
            #region Parameter validation

            if (model == null)
            {
                model = new EditSkillCategoryViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Image photo = null;
            if (model.Photo != null)
            {
                var bytes = model.Photo.Buffer;
                photo = _fileService.GetImage(bytes);
                if (photo.Height != ImageSizeConstant.StandardSkillCategoryImageSize ||
                    photo.Width != ImageSizeConstant.StandardSkillCategoryImageSize)
                {
                    ModelState.AddModelError($"{nameof(model)}.{nameof(model.Photo)}", HttpMessages.SkillCategoryImageSizeInvalid);
                    return BadRequest(ModelState);
                }
            }
            
            #endregion

            //Get SkillCategory
            var skillCategory = _dbContext.SkillCategories.Find(id);
            if (skillCategory == null)
                return NotFound();

            //Update information
            if (!string.IsNullOrWhiteSpace(model.Name))
                skillCategory.Name = model.Name;

            // Photo is defined.
            if (photo != null)
            {
                var relativeSkillCategoryImagePath = await _fileService.AddFileToDirectory(model.Photo.Buffer, _appPath.ProfileImage, null, CancellationToken.None);
                skillCategory.Photo = Url.Content(relativeSkillCategoryImagePath);
            }

            //Save change to db
            await _dbContext.SaveChangesAsync();

            return Ok(skillCategory);
        }

        /// <summary>
        ///     Delete skill category with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteSkillCategory([FromUri] int id)
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