using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Skill;
using ApiClientShared.ViewModel.SkillCategory;
using CvManagement.ViewModels.SkillCategory;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services.Businesses
{
    public interface ISkillCategoryService
    {
        #region Methods

        /// <summary>
        /// Add skill category asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SkillCategory> AddSkillCategoryAsync(AddSkillCategoryViewModel model, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Edit skill category asynchrously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SkillCategory> EditSkillCategoryAsync(int id, EditSkillCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete skill category asynchronously.
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
        Task<SearchResultViewModel<IList<SkillCategory>>> SearchSkillCategoriesAsync(SearchSkillCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for a project by using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SkillCategory> SearchSkillCategoryAsync(SearchSkillCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}