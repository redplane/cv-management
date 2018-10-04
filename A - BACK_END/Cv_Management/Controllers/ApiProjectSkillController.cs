using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.ProjectSkill;
using CvManagement.Interfaces.Services;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/project-skill")]
    public class ApiProjectSkillController : ApiController
    {
        #region Properties
        
        private readonly IDbService _dbService;

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Contructors

        public ApiProjectSkillController(IUnitOfWork unitOfWork,
            IDbService dbService)
        {
            _dbService = dbService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Mothods

        /// <summary>
        ///     get projects skill using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectSkillViewModel condition)
        {
            //Check null for condition model
            if (condition == null)
            {
                condition = new SearchProjectSkillViewModel();
                Validate(condition);
            }

            //Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Get project skills list
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

            //Result search 
            var result = new SearchResultViewModel<IList<ProjectSkill>>();
            result.Total = await projectSkills.CountAsync();
            
            //Do sort
            projectSkills = _dbService.Sort(projectSkills, SortDirection.Ascending, ProjectSkillSortProperty.ProjectId);

            //Do pagination
            projectSkills = _dbService.Paginate(projectSkills, condition.Pagination);

            result.Records = await projectSkills.ToListAsync();
            return Ok(result);
        }
        
        #endregion
    }
}