using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectResponsibility;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Services.Businesses
{
    public class ProjectResponsibilityService : IProjectResponsibilityService
    {
        #region Constructors

        public ProjectResponsibilityService(IUnitOfWork unitOfWork, IDbService dbService)
        {
            _unitOfWork = unitOfWork;
            _dbService = dbService;
        }

        #endregion

        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IDbService _dbService;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResultViewModel<IList<ProjectResponsibility>>>
            SearchProjectResponsibilitiesAsync(SearchProjectResponsibilityViewModel condition,
                CancellationToken cancellationToken = default(CancellationToken))
        {
            var projectResponsibilities = GetProjectResponsibilities(condition);
            var result = new SearchResultViewModel<IList<ProjectResponsibility>>();
            result.Total = await projectResponsibilities.CountAsync(cancellationToken);

            //Do sort
            projectResponsibilities =
                _dbService.Sort(projectResponsibilities, SortDirection.Ascending,
                    ProjectResponsibilitySortProperty.ProjectId);

            //Do paginatin
            projectResponsibilities = _dbService.Paginate(projectResponsibilities, condition.Pagination);

            result.Records = await projectResponsibilities.ToListAsync(cancellationToken);

            return result;
        }

        /// <summary>
        ///     Search for project responsibilities using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<ProjectResponsibility> GetProjectResponsibilities(
            SearchProjectResponsibilityViewModel condition)
        {
            //Get list projet responsibility
            var projectResponsibilities = _unitOfWork.ProjectResponsibilities.Search();
            if (condition.ProjectIds != null && condition.ProjectIds.Count > 0)
            {
                var projectIds = condition.ProjectIds.Where(x => x > 0).ToList();
                if (projectIds.Count > 0)
                    projectResponsibilities = projectResponsibilities.Where(x => projectIds.Contains(x.ProjectId));
            }

            if (condition.ResponsibilityIds != null && condition.ResponsibilityIds.Count > 0)
            {
                var responsibilityIds = condition.ResponsibilityIds.Where(x => x > 0).ToList();
                if (responsibilityIds.Count > 0)
                    projectResponsibilities =
                        projectResponsibilities.Where(x => responsibilityIds.Contains(x.ResponsibilityId));
            }

            return projectResponsibilities;
        }

        #endregion
    }
}