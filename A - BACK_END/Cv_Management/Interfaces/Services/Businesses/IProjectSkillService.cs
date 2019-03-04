using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectSkill;
using DbEntity.Models.Entities;

namespace CvManagement.Interfaces.Services.Businesses
{
    public interface IProjectSkillService
    {
        #region Methods
        
        /// <summary>
        /// Search for project-skill relationship.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResultViewModel<IList<ProjectSkill>>> SearchProjectSkillsAsync(SearchProjectSkillViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));
        
        #endregion
    }
}