using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Project;
using ApiClientShared.ViewModel.Responsibility;
using ApiClientShared.ViewModel.Skill;
using Cv_Management.Interfaces.Services;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace Cv_Management.Controllers
{
    [RoutePrefix("api/project")]
    public class ApiProjectController : ApiController
    {
        #region Contructors

        /// <summary>
        ///     Initialize controller with injectors
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbService"></param>
        public ApiProjectController(DbContext dbContext,
            IDbService dbService)
        {
            _dbContext = (CvManagementDbContext) dbContext;
            _dbService = dbService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Context to access to database
        /// </summary>
        private readonly CvManagementDbContext _dbContext;


        /// <summary>
        ///     Service to handler controller operation
        /// </summary>
        private readonly IDbService _dbService;

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


            var projects = _dbContext.Projects.AsQueryable();
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

            #region Search project skills & responsibilities.

            var skills = Enumerable.Empty<Skill>().AsQueryable();
            var projectSkills = Enumerable.Empty<ProjectSkill>().AsQueryable();

            if (condition.IncludeSkills)
            {
                skills = _dbContext.Skills.AsQueryable();
                projectSkills = _dbContext.ProjectSkills.AsQueryable();
            }


            var responsibilities = Enumerable.Empty<Responsibility>().AsQueryable();
            var projectResponsibilities = Enumerable.Empty<ProjectResponsibility>().AsQueryable();
            if (condition.IncludeResponsibilities)
            {
                responsibilities = _dbContext.Responsibilities.AsQueryable();
                projectResponsibilities = _dbContext.ProjectResponsibilities.AsQueryable();
            }

            var loadedProjects = from project in projects
                select new ProjectViewModel
                {
                    Id = project.Id,
                    UserId = project.UserId,
                    Name = project.Name,
                    Description = project.Description,
                    StartedTime = project.StartedTime,
                    FinishedTime = project.FinishedTime,
                    Skills = from projectSkill in projectSkills
                        from skill in skills
                        where projectSkill.ProjectId == project.Id && projectSkill.SkillId == skill.Id
                        select new SkillViewModel
                        {
                            Id = skill.Id,
                            Name = skill.Name,
                            CreatedTime = skill.CreatedTime,
                            LastModifiedTime = skill.LastModifiedTime
                        },
                    Responsibilities = from projectResponsibility in projectResponsibilities
                        from responsibility in responsibilities
                        where projectResponsibility.ProjectId == project.Id &&
                              projectResponsibility.ResponsibilityId == responsibility.Id
                        select new ResponsibilityViewModel
                        {
                            Id = responsibility.Id,
                            Name = responsibility.Name,
                            CreatedTime = responsibility.CreatedTime,
                            LastModifiedTime = responsibility.LastModifiedTime
                        }
                };

            #endregion

            var result = new SearchResultViewModel<IList<ProjectViewModel>>();
            result.Total = await projects.CountAsync();

            //Do sort
            loadedProjects = _dbService.Sort(loadedProjects, SortDirection.Ascending, ProjectSortProperty.Id);

            //Do Pagination
            loadedProjects = _dbService.Paginate(loadedProjects, condition.Pagination);

            result.Records = await loadedProjects.ToListAsync();
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
            var isExists = await _dbContext.Projects.AnyAsync(c => c.Name == model.Name);
            if (isExists)
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, "EXISTS_CODE_ERROR"));

            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                //Inial Project object
                var project = new Project();

                project.UserId = model.UserId;
                project.Name = model.Name;
                project.Description = model.Description;
                project.FinishedTime = model.FinishedTime;
                project.StartedTime = model.StatedTime;

                //Add project to database
                project = _dbContext.Projects.Add(project);
                if (model.SkillIds != null)
                {
                    #region add project skill

                    //check exists skill
                    var countSkills = await _dbContext.Skills.Where(c => model.SkillIds.Contains(c.Id)).CountAsync();
                    var isExistSkills = countSkills == model.SkillIds.Count;

                    if (!isExistSkills)
                        return NotFound();

                    //Insert to projectSkill table
                    foreach (var skillId in model.SkillIds)
                    {
                        var projectSkill = new ProjectSkill();

                        projectSkill.ProjectId = project.Id;
                        projectSkill.SkillId = skillId;

                        //Add to db context
                        _dbContext.ProjectSkills.Add(projectSkill);
                    }

                    #endregion
                }

                if (model.ResponsibilityIds != null)
                {
                    #region Project responsibilitis

                    // check exists responsibilities
                    var countRespon = await _dbContext.Responsibilities
                        .Where(c => model.ResponsibilityIds.Contains(c.Id)).CountAsync();
                    var isExistsRespon = countRespon == model.ResponsibilityIds.Count;

                    if (!isExistsRespon)
                        return NotFound();

                    //Insert project responsibility to db context
                    foreach (var responsibilityId in model.ResponsibilityIds)
                    {
                        var projectResponsibility = new ProjectResponsibility();

                        projectResponsibility.ProjectId = project.Id;
                        projectResponsibility.ResponsibilityId = responsibilityId;
                        projectResponsibility.CreatedTime = DateTime.UtcNow.ToOADate();

                        //Add to db context
                        _dbContext.ProjectResponsibilities.Add(projectResponsibility);
                    }

                    #endregion
                }


                //Save changes to database
                await _dbContext.SaveChangesAsync();

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
            var project = await _dbContext.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            //Update information
            project.UserId = model.UserId;
            project.Name = model.Name;
            project.Description = model.Description;
            project.FinishedTime = model.FinishedTime;
            project.StartedTime = model.StatedTime;

            // TODO: Update skills & responsibilities.

            #region  Update skills

            //Remove skill
            if (model.SkillIds != null)
            {
                var skills = _dbContext.Skills.Where(c => model.SkillIds.Contains(c.Id));
                var isExists = skills.Count() == model.SkillIds.Count;
                if (!isExists)
                    NotFound();

                foreach (var projectSkill in project.ProjectSkills.ToList())
                    project.ProjectSkills.Remove(projectSkill);
            }

            #endregion

            //Save changes to database
            await _dbContext.SaveChangesAsync();

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
            var project = _dbContext.Projects.Find(id);

            if (project == null)
                return NotFound();

            //Delete project from database
            _dbContext.Projects.Remove(project);

            //Save changes to database
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}