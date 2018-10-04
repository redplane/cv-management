using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Project;
using CvManagement.Interfaces.Services;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Controllers
{
    [RoutePrefix("api/project")]
    public class ApiProjectController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors
        /// </summary>
        /// <param name="dbService"></param>
        public ApiProjectController(IDbService dbService, IUnitOfWork unitOfWork)
        {
            _dbService = dbService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Service to handler controller operation
        /// </summary>
        private readonly IDbService _dbService;

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Methods

        /// <summary>
        ///     Get projects using specific conditions
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Search([FromBody] SearchProjectViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchProjectViewModel();
                Validate(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


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

            var result = new SearchResultViewModel<IList<Project>>();
            result.Total = await projects.CountAsync();

            //Do sort
            projects = _dbService.Sort(projects, SortDirection.Ascending, ProjectSortProperty.Id);

            //Do Pagination
            projects = _dbService.Paginate(projects, condition.Pagination);

            result.Records = await projects.ToListAsync();
            return Ok(result);
        }

        /// <summary>
        ///     Create Project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddProject([FromBody] AddProjectViewModel model)
        {
            if (model == null)
            {
                model = new AddProjectViewModel();
                Validate(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check exists project in database
            var projects = _unitOfWork.Projects.Search();
            projects = projects.Where(x => x.Name.Contains(model.Name));

            var project = await projects.FirstOrDefaultAsync();
            if (project != null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "EXISTS_CODE_ERROR"));

            var transaction = _unitOfWork.BeginTransactionScope();
            try
            {
                //Inial Project object
                project = new Project();

                project.UserId = model.UserId;
                project.Name = model.Name;
                project.Description = model.Description;
                project.FinishedTime = model.FinishedTime;
                project.StartedTime = model.StatedTime;

                //Add project to database
                _unitOfWork.Projects.Insert(project);
                if (model.SkillIds != null)
                {
                    #region add project skill

                    //check exists skill
                    var skills = _unitOfWork.Skills.Search();
                    var availableSkills = await skills.Where(x => model.SkillIds.Contains(x.Id)).ToListAsync();

                    // Insert to projectSkill table
                    foreach (var skill in availableSkills)
                    {
                        var projectSkill = new ProjectSkill();

                        projectSkill.ProjectId = project.Id;
                        projectSkill.SkillId = skill.Id;

                        //Add to db context
                        _unitOfWork.ProjectSkills.Insert(projectSkill);
                    }

                    #endregion
                }

                if (model.ResponsibilityIds != null)
                {
                    #region Project responsibilitis

                    // Find available responsibilities.
                    var responsibilities = _unitOfWork.Responsibilities.Search();
                    var availableResponsibilities = await responsibilities.Where(x => model.ResponsibilityIds.Contains(x.Id)).ToListAsync();

                    //Insert project responsibility to db context
                    foreach (var responsibility in availableResponsibilities)
                    {
                        var projectResponsibility = new ProjectResponsibility();

                        projectResponsibility.ProjectId = project.Id;
                        projectResponsibility.ResponsibilityId = responsibility.Id;
                        projectResponsibility.CreatedTime = DateTime.UtcNow.ToOADate();

                        //Add to db context
                        _unitOfWork.ProjectResponsibilities.Insert(projectResponsibility);
                    }

                    #endregion
                }

                //Save changes to database
                await _unitOfWork.CommitAsync();

                //Commit transaction
                transaction.Commit();

                //Success
                return Ok(project);
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Conflict();
            }
        }

        /// <summary>
        ///     Update Project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update([FromUri] int id, [FromBody] EditProjectViewModel model)
        {
            if (model == null)
            {
                model = new EditProjectViewModel();
                Validate(model);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Find  Project
            var projects = _unitOfWork.Projects.Search();
            var project = await projects.FirstOrDefaultAsync(x => x.Id == id);
            if (project == null)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    HttpMessages.ProjectNotFound));

            //Update information
            project.UserId = model.UserId;
            project.Name = model.Name;
            project.Description = model.Description;
            project.FinishedTime = model.FinishedTime;
            project.StartedTime = model.StatedTime;

            #region  Update skills

            // Update skills
            if (model.SkillIds != null)
            {
                // Get the list of available skills.
                var skills = _unitOfWork.Skills.Search();
                var availableSkills = await skills.Where(x => model.SkillIds.Contains(x.Id)).ToListAsync();

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
                var availableResponsibilities = await responsibilities.Where(x => model.ResponsibilityIds.Contains(x.Id)).ToListAsync();

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

            #endregion

            //Save changes to database
            await _unitOfWork.CommitAsync();

            return Ok(project);
        }

        /// <summary>
        ///     Delete project using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete([FromUri] int id)
        {
            //Find project in database
            var projects = _unitOfWork.Projects.Search();
            var project = await projects.FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
                return NotFound();

            //Delete project from database
            _unitOfWork.Projects.Remove(project);

            //Save changes to database
            await _unitOfWork.CommitAsync();

            return Ok();
        }

        #endregion
    }
}