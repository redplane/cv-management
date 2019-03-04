using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Skill;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services.Businesses
{
    public interface ISkillService
    {
        #region Methods

        /// <summary>
        /// Add project asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Skill> AddSkillAsync(AddSkillViewModel model, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Edit project asynchrously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Skill> EditSkillAsync(int id, EditSkillViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete project asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteSkillAsync(int id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResultViewModel<IList<Skill>>> SearchSkillsAsync(SearchSkillViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for a project by using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Skill> SearchSkillAsync(SearchSkillViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}