using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;
using System.Web.UI.WebControls;
using ApiClientShared.Enums;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.User;
using AutoMapper;
using Cv_Management.Interfaces.Services;
using Cv_Management.Models;
using Cv_Management.Models.Operations;
using Cv_Management.ViewModels;
using Cv_Management.ViewModels.User;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;
using Microsoft.EntityFrameworkCore;

namespace Cv_Management.Services
{
    public class UserService : IUserService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Profile caching service (email - profile model)
        /// </summary>
        private readonly IValueCacheService<string, ProfileModel> _profileCacheService;

        /// <summary>
        /// Profile service (later will be merged to this class)
        /// </summary>
        private readonly IProfileService _profileService;

        /// <summary>
        /// Mapper instance.
        /// </summary>
        private readonly IMapper _mapper;

        private readonly IDbService _dbService;

        private readonly IFileService _fileService;

        private readonly AppPathModel _appPath;

        private readonly UrlHelper _urlHelper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="profileCacheService"></param>
        /// <param name="profileService"></param>
        /// <param name="mapper"></param>
        /// <param name="dbService"></param>
        /// <param name="fileService"></param>
        /// <param name="appPath"></param>
        public UserService(IUnitOfWork unitOfWork, 
            IValueCacheService<string, ProfileModel> profileCacheService, 
            IProfileService profileService, IMapper mapper, 
            UrlHelper urlHelper,
            IDbService dbService,
            IFileService fileService, AppPathModel appPath)
        {
            _unitOfWork = unitOfWork;
            _profileCacheService = profileCacheService;
            _profileService = profileService;
            _mapper = mapper;
            _dbService = dbService;
            _fileService = fileService;
            _urlHelper = urlHelper;
            _appPath = appPath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="loginModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ProfileModel> LoginAsync(LoginViewModel loginModel, CancellationToken cancellationToken)
        {
            // Find user by using email from cache first.
            var profile = _profileCacheService.Read(loginModel.Email);

            // Hash password by using md5.
            var hashedPassword = _profileService.HashPassword(loginModel.Password);

            // No profile has been found from cache. Find user from database.
            if (profile == null || string.IsNullOrWhiteSpace(profile.Password))
            {
                var users = _unitOfWork.Users.Search();
                
                users = users.Where(x =>
                    x.Email.Equals(loginModel.Email, StringComparison.InvariantCultureIgnoreCase) &&
                    x.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase) && x.Status == UserStatuses.Active);

                var user = await users.FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                    return null;

                profile = _mapper.Map<ProfileModel>(user);
                return profile;
            }

            if (!profile.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return profile;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> AddUserAsync(AddUserViewModel model, CancellationToken cancellationToken)
        {
            // Check user duplicate.
            var users = _unitOfWork.Users.Search();
            users = users.Where(x => x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));
            var addedUser = await users.FirstOrDefaultAsync(cancellationToken);
            if (addedUser != null)
                throw new Exception(HttpMessages.UserAlreadyExist);

            var user = new User();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Birthday = model.Birthday;
            if (model.Photo != null)
                user.Photo = Convert.ToBase64String(model.Photo.Buffer);
            user.Role = model.Role;
            user.Email = model.Email;
            user.Password = model.Password;

            _unitOfWork.Users.Insert(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async  Task<User> EditUserAsync(int id, EditUserViewModel model, CancellationToken cancellationToken = default (CancellationToken))
        {
            //Find user
            var loadUserCondition = new SearchUserViewModel();
            loadUserCondition.Ids = new HashSet<int>();
            loadUserCondition.Ids.Add(id);

            var user = await GetUserAsync(loadUserCondition, cancellationToken);
            if (user == null)
                throw new Exception(HttpMessages.UserNotFound);

            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (model.Birthday != null)
                user.Birthday = model.Birthday.Value;

            // Photo is defined. Save photo to path.
            if (model.Photo != null)
            {
                var relativeProfileImagePath = await _fileService.AddFileToDirectory(model.Photo.Buffer,
                    _appPath.ProfileImage, null, CancellationToken.None);
                
                user.Photo = _urlHelper.Content(relativeProfileImagePath);
            }

            //Save to database
            await _unitOfWork.CommitAsync();
            return user;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default (CancellationToken))
        {
            // Find users.
            var loadUserCondition = new SearchUserViewModel();
            loadUserCondition.Ids = new HashSet<int>();
            loadUserCondition.Ids.Add(id);

            var user = await GetUserAsync(loadUserCondition, cancellationToken);
            if (user == null)
                throw new Exception(HttpMessages.UserNotFound);

            user.Status = UserStatuses.Disabled;
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Register user using specific information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<RegisterUserResult> RegisterUserAsync(RegisterViewModel model, CancellationToken cancellationToken)
        {
            // Check user duplicate.
            var users = _unitOfWork.Users.Search();
            var user = await users.FirstOrDefaultAsync(x =>
                x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase), cancellationToken);

            if (user != null)
                throw new Exception(HttpMessages.UserAlreadyExist);

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                #region Initialize user

                user = new User();
                user.Email = model.Email;
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;

                // Mark account as pending.
                user.Status = UserStatuses.Pending;

                // Hash the password for user protection.
                user.Password = _profileService.HashPassword(model.Password);

                _unitOfWork.Users.Insert(user);

                #endregion

                #region Initialize token

                // Find list of profile activation tokens.
                var profileActivationTokens = _unitOfWork.ProfileActivationTokens.Search();
                profileActivationTokens = profileActivationTokens.Where(x =>
                    x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));

                var profileActivationToken = new ProfileActivationToken();
                profileActivationToken.Email = model.Email;
                profileActivationToken.Token = Guid.NewGuid().ToString("D");
                profileActivationToken.CreatedTime = 0;

                // Delete previous activation token and add the new one.
                _unitOfWork.ProfileActivationTokens.Remove(profileActivationTokens);
                _unitOfWork.ProfileActivationTokens.Insert(profileActivationToken);

                #endregion

                var registerResult = new RegisterUserResult();
                registerResult.Email = user.Email;
                registerResult.Token = profileActivationToken.Token;

                await _unitOfWork.CommitAsync();
                transactionScope.Complete();

                return registerResult;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResultViewModel<IList<User>>> GetUsersAsync(SearchUserViewModel condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var users = FindUsers(condition);

            // Initialize search result.
            var searchResult = new SearchResultViewModel<IList<User>>();

            // Count total users.
            searchResult.Total = await users.CountAsync(cancellationToken);

            // Get pagination information.
            var pagination = condition.Pagination;
            users = _dbService.Paginate(users, pagination);

            searchResult.Records = await users.ToListAsync(cancellationToken);
            return searchResult;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> GetUserAsync(SearchUserViewModel condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await FindUsers(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Search users by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<User> FindUsers(SearchUserViewModel condition)
        {
            // Get list of users.
            var users = _unitOfWork.Users.Search();

            // Ids are defined.
            if (condition.Ids != null)
            {
                var ids = condition.Ids.Where(c => c > 0).ToList();
                if (ids.Count > 0)
                    users = users.Where(c => condition.Ids.Contains(c.Id));
            }

            // Last names are defined.
            if (condition.LastNames != null)
            {
                var lastNames = condition.LastNames.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (lastNames.Count > 0)
                    users = users.Where(c => lastNames.Contains(c.LastName));
            }

            // First names are defined.
            if (condition.FirstNames != null)
            {
                var firstNames = condition.FirstNames.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (firstNames.Count > 0)
                    users = users.Where(c => firstNames.Contains(c.FirstName));
            }

            // Birthday range is defined.
            if (condition.Birthday != null)
            {
                var birthday = condition.Birthday;
                if (birthday.From != null)
                    users = users.Where(c => c.Birthday >= birthday.From);

                if (birthday.To != null)
                    users = users.Where(user => user.Birthday <= birthday.To);
            }

            // Statuses are defined.
            if (condition.Statuses != null)
            {
                var statuses = condition.Statuses.Where(x => Enum.IsDefined(typeof(UserRoles), x)).ToList();
                if (statuses.Count > 0)
                    users = users.Where(x => statuses.Contains(x.Status));
            }

            // Roles are defined.
            if (condition.Roles != null)
            {
                var roles = condition.Roles.Where(x => Enum.IsDefined(typeof(UserRoles), x)).ToList();
                if (roles.Count > 0)
                    users = users.Where(x => roles.Contains(x.Role));
            }

            return users;
        }


        #endregion
    }
}