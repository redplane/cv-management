using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Hobby;
using ApiClientShared.ViewModel.Project;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services.Businesses
{
    public interface IHobbyService
    {
        #region Methods

        /// <summary>
        /// Add project asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Hobby> AddHobbyAsync(AddHobbyViewModel model, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Edit project asynchrously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Hobby> EditHobbyAsync(int id, EditHobbyViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete project asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteHobbyAsync(int id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResultViewModel<IList<Hobby>>> SearchHobbiesAsync(SearchHobbyViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for a project by using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Hobby> SearchHobbyAsync(SearchHobbyViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}