﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Responsibility;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services.Businesses
{
    public interface IResponsibilityService
    {
        /// <summary>
        ///     Add project asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Responsibility> AddResponsibilityAsync(AddResponsibilityViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Edit project asynchrously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Responsibility> EditResponsibilityAsync(int id, EditResponsibilityViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Delete project asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteResponsibilityAsync(int id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResultViewModel<IList<Responsibility>>> SearchResponsibilitiesAsync(
            SearchResponsibilityViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search for a project by using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Responsibility> SearchResponsibilityAsync(SearchResponsibilityViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}