using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectSkill;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Services.Businesses
{
    public class ProjectSkillService : IProjectSkillService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IDbService _dbService;

        #endregion

        #region Constructor

        public ProjectSkillService(IUnitOfWork unitOfWork, IDbService dbService)
        {
            _unitOfWork = unitOfWork;
            _dbService = dbService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual  async Task<SearchResultViewModel<IList<ProjectSkill>>> SearchProjectSkillsAsync(SearchProjectSkillViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var projectSkills = GetProjectSkills(condition);
            var result = new SearchResultViewModel<IList<ProjectSkill>>();
            result.Total = await projectSkills.CountAsync(cancellationToken);

            //Do sort
            projectSkills = _dbService.Sort(projectSkills, SortDirection.Ascending, ProjectSortProperty.Id);

            //Do Pagination
            result.Records = await _dbService.Paginate(projectSkills, condition.Pagination).ToListAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Get project skill using specific conditions.
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<ProjectSkill> GetProjectSkills(SearchProjectSkillViewModel condition)
        {
            var projectSkills = _unitOfWork.ProjectSkills.Search();
            
            if (condition.ProjectIds != null)
            {
                var projectIds = condition.ProjectIds.Where(x => x > 0).ToList();
                if (projectIds.Count > 0)
                    projectSkills = projectSkills.Where(x => projectIds.Contains(x.ProjectId));
            }

            if (condition.SkillIds != null)
            {
                var skillIds = condition.SkillIds.Where(x => x > 0).ToList();
                if (skillIds.Count > 0)
                    projectSkills = projectSkills.Where(x => skillIds.Contains(x.SkillId));
            }

            return projectSkills;
        }

        #endregion
    }
}