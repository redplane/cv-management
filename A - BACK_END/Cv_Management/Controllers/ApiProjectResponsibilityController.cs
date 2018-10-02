using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectResponsibility;
using Cv_Management.Interfaces.Services;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/project-responsibility")]
    public class ApiProjectResponsibilityController : ApiController
    {
        #region Contructors

        public ApiProjectResponsibilityController(IUnitOfWork unitOfWork,
            IDbService dbService)
        {
            _dbService = dbService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get project responsibility using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectResponsibilityViewModel condition)
        {
            //Check null for model 
            if (condition == null)
            {
                condition = new SearchProjectResponsibilityViewModel();
                Validate(condition);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

            var result = new SearchResultViewModel<IList<ProjectResponsibility>>();
            result.Total = await projectResponsibilities.CountAsync();

            //Do sort
            projectResponsibilities =
                _dbService.Sort(projectResponsibilities, SortDirection.Ascending,
                    ProjectResponsibilitySortProperty.ProjectId);

            //Do paginatin
            projectResponsibilities = _dbService.Paginate(projectResponsibilities, condition.Pagination);

            result.Records = await projectResponsibilities.ToListAsync();

            return Ok(result);
        }

        #endregion

        #region properties

        private readonly IDbService _dbService;

        private readonly IUnitOfWork _unitOfWork;

        #endregion
    }
}