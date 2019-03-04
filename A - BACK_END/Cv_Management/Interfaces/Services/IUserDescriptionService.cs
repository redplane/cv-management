using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.UserDescription;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services
{
    public interface IUserDescriptionService
    {
        #region Methods

        /// <summary>
        /// Add user description using specific information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserDescription> AddUserDescriptionAsync(AddUserDescriptionViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete user description asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteUserDescriptionAsync(int id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Edit user description using specific information asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserDescription> EditUserDescriptionAsync(int id, EditUserDescriptionViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search user description using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResultViewModel<IList<UserDescription>>> SearchUserDescriptionsAsync(
            SearchUserDescriptionViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search single user description using condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserDescription> SearchUserDescription(SearchUserDescriptionViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}