using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.User;
using CvManagement.Models;
using CvManagement.Models.Operations;
using CvManagement.ViewModels;
using CvManagement.ViewModels.User;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services
{
    public interface IUserService
    {
        #region Methods

        /// <summary>
        /// Find user information using login information.
        /// </summary>
        /// <param name="loginModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ProfileModel> LoginAsync(LoginViewModel loginModel, CancellationToken cancellationToken);

        /// <summary>
        /// Add user to service.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> AddUserAsync(AddUserViewModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Edit user using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> EditUserAsync(int id, EditUserViewModel model, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete user by using his/her id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteUserAsync(int id, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Register user to system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<RegisterUserResult> RegisterUserAsync(RegisterViewModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Get user using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResultViewModel<IList<User>>> GetUsersAsync(SearchUserViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search user using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> GetUserAsync(SearchUserViewModel condition, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

    }
}