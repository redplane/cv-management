using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Routing;
using System.Web.UI.WebControls;
using ApiClientShared.Constants;
using ApiClientShared.Enums;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.SkillCategory;
using AutoMapper;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using CvManagement.Models;
using CvManagement.ViewModels.SkillCategory;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Services.Businesses
{
    public class SkillCategoryService : ISkillCategoryService
    {
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

        private readonly IProfileService _profileService;

        private readonly HttpRequestMessage _httpRequestMessage;

        private readonly UrlHelper _urlHelper;

        #endregion

        #region Constructor

        public SkillCategoryService(IDbService dbService, IMapper mapper, IFileService fileService, AppPathModel appPath, IUnitOfWork unitOfWork, HttpRequestMessage httpRequestMessage, UrlHelper urlHelper)
        {
            _dbService = dbService;
            _mapper = mapper;
            _appPath = appPath;
            _unitOfWork = unitOfWork;
            _httpRequestMessage = httpRequestMessage;
            _urlHelper = urlHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SkillCategory> AddSkillCategoryAsync(AddSkillCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get request profile.
            var profile = _profileService.GetProfile(_httpRequestMessage);
            var userId = model.UserId;

            if (profile.Role != UserRoles.Admin)
                userId = profile.Id;

            var skillCategories = _unitOfWork.SkillCategories.Search();
            var skillCategory =
                await skillCategories.FirstOrDefaultAsync(x => x.Name.Equals(model.Name) && x.UserId == userId, CancellationToken.None);

            if (skillCategory != null)
                throw new HttpException((int)HttpStatusCode.Conflict, HttpMessages.SkillCategoryAlreadyAvailable);

            skillCategory = new SkillCategory();
            skillCategory.Name = model.Name;
            skillCategory.UserId = model.UserId;

            // Photo is defined. Save photo to path.
            if (model.Photo != null)
            {
                var relativeProfileImagePath = await _fileService.AddFileToDirectory(model.Photo.Buffer,
                    _appPath.ProfileImage, null, CancellationToken.None);

                skillCategory.Photo = _urlHelper.Content(relativeProfileImagePath);
            }

            if (model.Photo != null)
                skillCategory.Photo = Convert.ToBase64String(model.Photo.Buffer);
            skillCategory.CreatedTime = DateTime.Now.ToOADate();

            //Save to db context
            _unitOfWork.SkillCategories.Insert(skillCategory);

            //save change to db
            await _unitOfWork.CommitAsync();
            return skillCategory;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SkillCategory> EditSkillCategoryAsync(int id, EditSkillCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //Get SkillCategory
            var skillCategories = _unitOfWork.SkillCategories.Search();
            skillCategories = skillCategories.Where(x => x.Id == id);
            var skillCategory = await skillCategories.FirstOrDefaultAsync(cancellationToken);

            if (skillCategory == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpMessages.SkillCategoryNotFound);

            //Update information
            if (!string.IsNullOrWhiteSpace(model.Name))
                skillCategory.Name = model.Name;
            
            // Photo is defined.
            if (model.Photo != null)
            {
                var relativeSkillCategoryImagePath = await _fileService.AddFileToDirectory(model.Photo.Buffer, _appPath.ProfileImage, null, CancellationToken.None);
                skillCategory.Photo = _urlHelper.Content(relativeSkillCategoryImagePath);
            }

            //Save change to db
            await _unitOfWork.CommitAsync();
            return skillCategory;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteSkillAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Get SkillCategory
            var skillCategories = _unitOfWork.SkillCategories.Search();
            skillCategories = skillCategories.Where(x => x.Id == id);
            var skillCategory = await skillCategories.FirstOrDefaultAsync();

            if (skillCategory == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpMessages.SkillCategoryNotFound);

            _unitOfWork.SkillCategories.Remove(skillCategory);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResultViewModel<IList<SkillCategory>>> SearchSkillCategoriesAsync(SearchSkillCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var skillCategories = GetSkillCategories(condition);
            // Get offline skill categories.
            var loadSkillCategoriesResult = new SearchResultViewModel<IList<SkillCategory>>();
            loadSkillCategoriesResult.Total = await skillCategories.CountAsync(cancellationToken);

            // Do sorting.
            skillCategories = _dbService.Sort(skillCategories, SortDirection.Ascending,
                SkillCategorySortProperty.Id);

            // Do paging.
            skillCategories = _dbService.Paginate(skillCategories, condition.Pagination);
            loadSkillCategoriesResult.Records = await skillCategories.ToListAsync(cancellationToken);
            return loadSkillCategoriesResult;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SkillCategory> SearchSkillCategoryAsync(SearchSkillCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetSkillCategories(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Get skill categories using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<SkillCategory> GetSkillCategories(SearchSkillCategoryViewModel condition)
        {
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

            return skillCategories;
        }

        #endregion
    }
}