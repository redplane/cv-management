using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using ApiClientShared.Enums;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.UserDescription;
using CvManagement.Interfaces.Services;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Services
{
    public class UserDescriptionService : IUserDescriptionService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IDbService _dbService;

        #endregion

        #region Constructors

        public UserDescriptionService(IUnitOfWork unitOfWork, IDbService dbService)
        {
            _unitOfWork = unitOfWork;
            _dbService = dbService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<UserDescription> AddUserDescriptionAsync(AddUserDescriptionViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (model.UserId == null || model.UserId < 1)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpMessages.UserNotFound);

            var users = _unitOfWork.Users.Search();
            users = users.Where(x => x.Id == model.UserId && x.Status == UserStatuses.Active);

            var user = await users.FirstOrDefaultAsync(cancellationToken);
            if (user == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpMessages.UserNotFound);

            // Add user description into database.
            var userDescription = new UserDescription();
            userDescription.UserId = model.UserId.Value;
            userDescription.Description = model.Description;

            // Add the description into database.
            _unitOfWork.UserDescriptions.Insert(userDescription);

            // Save changes into database.
            await _unitOfWork.CommitAsync();
            return userDescription;
        }

        /// <summary>
        /// Find and delete user description.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteUserDescriptionAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find user description.
            var loadUserDescriptionCondition = new SearchUserDescriptionViewModel();
            loadUserDescriptionCondition.Ids = new HashSet<int>();
            loadUserDescriptionCondition.Ids.Add(id);

            // Get user description.
            var userDescription =
                await GetUserDescriptions(loadUserDescriptionCondition).FirstOrDefaultAsync(cancellationToken);

            if (userDescription == null)
                throw new HttpException((int)HttpStatusCode.NotFound, HttpMessages.UserDescriptionNotFoud);

            _unitOfWork.UserDescriptions.Remove(userDescription);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<UserDescription> EditUserDescriptionAsync(int id, EditUserDescriptionViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find user description.
            var loadUserDescriptionCondition = new SearchUserDescriptionViewModel();
            loadUserDescriptionCondition.Ids = new HashSet<int>();
            loadUserDescriptionCondition.Ids.Add(id);

            // Get user description.
            var userDescription =
                await GetUserDescriptions(loadUserDescriptionCondition).FirstOrDefaultAsync(cancellationToken);

            if (userDescription == null)
                throw new HttpException((int) HttpStatusCode.NotFound, HttpMessages.UserDescriptionNotFoud);

            userDescription.Description = model.Description;
            await _unitOfWork.CommitAsync();
            return userDescription;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResultViewModel<IList<UserDescription>>> SearchUserDescriptionsAsync(SearchUserDescriptionViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var userDescriptions = GetUserDescriptions(condition);
            var loadUserDescriptionResult = new SearchResultViewModel<IList<UserDescription>>();
            loadUserDescriptionResult.Total = await userDescriptions.CountAsync(cancellationToken);

            // Do sorting.
            userDescriptions =
                _dbService.Sort(userDescriptions, SortDirection.Ascending, UserDescriptionSortProperty.Id);

            // Do pagination.
            userDescriptions = _dbService.Paginate(userDescriptions, condition.Pagination);

            loadUserDescriptionResult.Records = await userDescriptions.ToListAsync(cancellationToken);

            return loadUserDescriptionResult;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<UserDescription> SearchUserDescription(SearchUserDescriptionViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetUserDescriptions(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Search for user descriptions using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<UserDescription> GetUserDescriptions(SearchUserDescriptionViewModel condition)
        {
            var userDescriptions = _unitOfWork.UserDescriptions.Search();

            // Ids list is defined.
            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                var ids = condition.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    userDescriptions = userDescriptions.Where(userDescription => ids.Contains(userDescription.Id));
            }

            // User ids list is defined.
            if (condition.UserIds != null && condition.UserIds.Count > 0)
            {
                var userIds = condition.UserIds.Where(x => x > 0).ToList();
                if (userIds.Count > 0)
                    userDescriptions =
                        userDescriptions.Where(userDescription => userIds.Contains(userDescription.UserId));
            }

            return userDescriptions;
        }

        #endregion
    }
}