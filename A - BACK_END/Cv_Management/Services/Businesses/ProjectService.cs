using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using ApiClientShared.Enums;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Project;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Services.Businesses
{
    public class ProjectService : IProjectService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IDbService _dbService;

        private readonly HttpRequestMessage _httpRequestMessage;

        private readonly IProfileService _profileService;

        #endregion

        #region Constructor

        public ProjectService(IUnitOfWork unitOfWork, IDbService dbService, HttpRequestMessage httpRequestMessage, IProfileService profileService)
        {
            _unitOfWork = unitOfWork;
            _dbService = dbService;
            _httpRequestMessage = httpRequestMessage;
            _profileService = profileService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Project> AddProjectAsync(AddProjectViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Project name cannot be duplicated
            var projects = _unitOfWork.Projects.Search();
            projects = projects.Where(x => x.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase));

            var project = await projects.FirstOrDefaultAsync(cancellationToken);
            if (project != null)
                throw new HttpException((int) HttpStatusCode.Conflict, HttpMessages.ProjectAlreadyExist);

            project = new Project(model.UserId, model.Name, model.Description, model.StatedTime, model.FinishedTime);
            _unitOfWork.Projects.Insert(project);
            await _unitOfWork.CommitAsync();
            return project;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Project> EditProjectAsync(int id, EditProjectViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var loadProjectCondition = new SearchProjectViewModel();
            loadProjectCondition.Ids = new HashSet<int>();
            loadProjectCondition.Ids.Add(id);
            
            var profile = _profileService.GetProfile(_httpRequestMessage);
            if (profile == null)
                throw new HttpException((int) HttpStatusCode.Forbidden, HttpMessages.ActionIsForbidden);

            // Non admin user can only edit his/her project.
            if (profile.Role != UserRoles.Admin)
            {
                loadProjectCondition.UserIds = new HashSet<int>();
                loadProjectCondition.UserIds.Add(profile.Id);
            }

            var project = await GetProjects(loadProjectCondition).FirstOrDefaultAsync(cancellationToken);
            if (project == null)
                throw new HttpException((int) HttpStatusCode.NotFound, HttpMessages.ProjectNotFound);

            // Transaction begin.
            var transaction = _unitOfWork.BeginTransactionScope();

            //Update information
            project.UserId = model.UserId;
            project.Name = model.Name;
            project.Description = model.Description;
            project.FinishedTime = model.FinishedTime;
            project.StartedTime = model.StatedTime;

            // Update skills
            if (model.SkillIds != null)
            {
                // Get the list of available skills.
                var skills = _unitOfWork.Skills.Search();
                var availableSkills = await skills.Where(x => model.SkillIds.Contains(x.Id)).ToListAsync(cancellationToken);

                var projectSkills = _unitOfWork.ProjectSkills.Search();
                projectSkills = projectSkills.Where(x => x.ProjectId == id);
                _unitOfWork.ProjectSkills.Remove(projectSkills);

                foreach (var availableSkill in availableSkills)
                {
                    var projectSkill = new ProjectSkill();
                    projectSkill.ProjectId = id;
                    projectSkill.SkillId = availableSkill.Id;

                    _unitOfWork.ProjectSkills.Insert(projectSkill);
                }
            }

            // Update responsibilities
            if (model.ResponsibilityIds != null)
            {
                // Get the list of available skills.
                var responsibilities = _unitOfWork.Responsibilities.Search();
                var availableResponsibilities = await responsibilities.Where(x => model.ResponsibilityIds.Contains(x.Id)).ToListAsync(cancellationToken);

                var projectResponsibilities = _unitOfWork.ProjectResponsibilities.Search();
                projectResponsibilities = projectResponsibilities.Where(x => x.ProjectId == id);
                _unitOfWork.ProjectResponsibilities.Remove(projectResponsibilities);

                foreach (var availableResponsibility in availableResponsibilities)
                {
                    var projectResponsibility = new ProjectResponsibility();
                    projectResponsibility.ProjectId = id;
                    projectResponsibility.ResponsibilityId = availableResponsibility.Id;

                    _unitOfWork.ProjectResponsibilities.Insert(projectResponsibility);
                }
            }

            await _unitOfWork.CommitAsync();
            transaction.Commit();
            return project;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteProjectAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var projectResponsibilities = _unitOfWork.ProjectResponsibilities.Search();
            var projectSkills = _unitOfWork.ProjectSkills.Search();
            var projects = _unitOfWork.Projects.Search();

            projects = projects.Where(x => x.Id == id);
            if (!await projects.AnyAsync(cancellationToken))
                throw new HttpException((int) HttpStatusCode.NotFound, HttpMessages.ProjectNotFound);

            projectResponsibilities = projectResponsibilities.Where(x => x.ProjectId == id);
            projectSkills = projectSkills.Where(x => x.ProjectId == id);

            var transaction = _unitOfWork.BeginTransactionScope();
            
            _unitOfWork.ProjectSkills.Remove(projectSkills);
            _unitOfWork.ProjectResponsibilities.Remove(projectResponsibilities);
            _unitOfWork.Projects.Remove(projects);

            await _unitOfWork.CommitAsync();
            transaction.Commit();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResultViewModel<IList<Project>>> SearchProjectsAsync(SearchProjectViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var projects = GetProjects(condition);
            var result = new SearchResultViewModel<IList<Project>>();
            result.Total = await projects.CountAsync(cancellationToken);

            //Do sort
            projects = _dbService.Sort(projects, SortDirection.Ascending, ProjectSortProperty.Id);

            //Do Pagination
            result.Records = await _dbService.Paginate(projects, condition.Pagination).ToListAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Project> SearchProjectAsync(SearchProjectViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var project = await GetProjects(condition).FirstOrDefaultAsync(cancellationToken);
            return project;
        }
        
        /// <summary>
        /// Get projects by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<Project> GetProjects(SearchProjectViewModel condition)
        {
            var projects = _unitOfWork.Projects.Search();
            if (condition.Ids != null)
            {
                var ids = condition.Ids.Where(x => x > 0).ToList();
                if (ids.Count > 0)
                    projects = projects.Where(x => ids.Contains(x.Id));
            }

            if (condition.Names != null)
            {
                var names = condition.Names.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (names.Count > 0)
                    projects = projects.Where(c => condition.Names.Contains(c.Name));
            }

            if (condition.UserIds != null)
            {
                var userIds = condition.UserIds.Where(c => c > 0).ToList();
                if (userIds.Count > 0)
                    projects = projects.Where(c => condition.UserIds.Contains(c.UserId));
            }

            if (condition.StartedTime != null)
                projects = projects.Where(c => c.StartedTime >= condition.StartedTime.From
                                               && c.StartedTime <= condition.StartedTime.To);

            if (condition.FinishedTime != null)
                projects = projects.Where(c => c.FinishedTime >= condition.FinishedTime.From
                                               && c.FinishedTime <= condition.FinishedTime.To);

            return projects;
        }
        
        #endregion
    }
}