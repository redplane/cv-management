using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Project;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services.Businesses
{
    public interface IProjectService
    {
        #region Methods

        /// <summary>
        /// Add project asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Project> AddProjectAsync(AddProjectViewModel model, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// Edit project asynchrously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Project> EditProjectAsync(int id, EditProjectViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete project asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteProjectAsync(int id, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResultViewModel<IList<Project>>> SearchProjectsAsync(SearchProjectViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for a project by using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Project> SearchProjectAsync(SearchProjectViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}