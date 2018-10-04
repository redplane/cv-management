using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Constants;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.SkillCategory;
using AutoMapper;
using CvManagement.Interfaces.Services;
using CvManagement.Models;
using CvManagement.ViewModels.SkillCategory;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Image = System.Drawing.Image;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/skill-category")]
    public class ApiSkillCategoryController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="dbService"></param>
        /// <param name="mapper"></param>
        /// <param name="fileService"></param>
        /// <param name="appPath"></param>
        public ApiSkillCategoryController(IUnitOfWork unitOfWork, IDbService dbService, IMapper mapper, IFileService fileService, AppPathModel appPath)
        {
            _unitOfWork = unitOfWork;
            _dbService = dbService;
            _mapper = mapper;
            _fileService = fileService;
            _appPath = appPath;
        }

        #endregion

        #region Properties

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

        private readonly IUnitOfWork _unitOfWork;

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
            var skillCategories = _unitOfWork.SkillCategories.Search();

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
            var loadSkillCategoryResult = new SearchResultViewModel<IEnumerable<SkillCategory>>();
            loadSkillCategoryResult.Total = await skillCategories.CountAsync();

            // Do sorting.
            skillCategories = _dbService.Sort(skillCategories, SortDirection.Ascending,
                SkillCategorySortProperty.Id);

            // Do paging.
            skillCategories = _dbService.Paginate(skillCategories, condition.Pagination);
            loadSkillCategoryResult.Records = await skillCategories.ToListAsync();

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

            var skillCategories = _unitOfWork.SkillCategories.Search();
            var skillCategory =
                await skillCategories.FirstOrDefaultAsync(x => x.Name.Equals(model.Name) && x.UserId == model.UserId);
            if (skillCategory != null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict,
                    HttpMessages.SkillCategoryAlreadyAvailable));

            skillCategory = new SkillCategory();
            skillCategory.Name = model.Name;
            skillCategory.UserId = model.UserId;
            if (model.Photo != null)
                skillCategory.Photo = Convert.ToBase64String(model.Photo.Buffer);
            skillCategory.CreatedTime = DateTime.Now.ToOADate();

            //Save to db context
            _unitOfWork.SkillCategories.Insert(skillCategory);

            //save change to db
            await _unitOfWork.CommitAsync();

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
            var skillCategories = _unitOfWork.SkillCategories.Search();
            skillCategories = skillCategories.Where(x => x.Id == id);
            var skillCategory = await skillCategories.FirstOrDefaultAsync();

            if (skillCategory == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, HttpMessages.SkillCategoryNotFound));

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
            await _unitOfWork.CommitAsync();

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
            //Get SkillCategory
            var skillCategories = _unitOfWork.SkillCategories.Search();
            skillCategories = skillCategories.Where(x => x.Id == id);
            var skillCategory = await skillCategories.FirstOrDefaultAsync();

            if (skillCategory == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, HttpMessages.SkillCategoryNotFound));

            _unitOfWork.SkillCategories.Remove(skillCategory);
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        #endregion
    }
}