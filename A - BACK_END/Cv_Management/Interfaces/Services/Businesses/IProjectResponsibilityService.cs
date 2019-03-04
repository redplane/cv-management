using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectResponsibility;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services.Businesses
{
    public interface IProjectResponsibilityService
    {
        #region Methods

        /// <summary>
        /// Search for a project by using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResultViewModel<IList<ProjectResponsibility>>> SearchProjectResponsibilitiesAsync(SearchProjectResponsibilityViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}