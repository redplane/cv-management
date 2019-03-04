using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using ApiClientShared.Enums.SortProperties;
using ApiClientShared.Resources;
using ApiClientShared.ViewModel;
using ApiClientShared.ViewModel.Skill;
using CvManagement.Interfaces.Services;
using CvManagement.Interfaces.Services.Businesses;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvManagement.Services.Businesses
{
    public class SkillService : ISkillService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IDbService _dbService;
            
        #endregion

        #region Constructor

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="dbService"></param>
        public SkillService(IUnitOfWork unitOfWork, IDbService dbService)
        {
            _unitOfWork = unitOfWork;
            _dbService = dbService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Skill> AddSkillAsync(AddSkillViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            var skills = _unitOfWork.Skills.Search();
            skills = skills.Where(x => x.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase));

            var skill = await skills.FirstOrDefaultAsync(cancellationToken);

            if (skill != null)
                throw new HttpException((int) HttpStatusCode.Conflict, HttpMessages.SkillIsDuplicate);

            //Inital skill object
            skill = new Skill();
            skill.Name = model.Name;
            skill.CreatedTime = DateTime.Now.ToOADate();

            //add skill to database
            _unitOfWork.Skills.Insert(skill);

            //save changes to database
            await _unitOfWork.CommitAsync();
            return skill;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Skill> EditSkillAsync(int id, EditSkillViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {

            //Find skill in database
            var skills = _unitOfWork.Skills.Search();
            var skill = await skills.FirstOrDefaultAsync(x => x.Id == id);
            if (skill == null)
                throw new HttpException((int) HttpStatusCode.NotFound, HttpMessages.SkillNotFound);

            //Update information
            skill.Name = model.Name;
            skill.LastModifiedTime = DateTime.Now.ToOADate();

            //Save changes to database
            await _unitOfWork.CommitAsync();
            return skill;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteSkillAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Find skill in database
            var skills = _unitOfWork.Skills.Search();

            var skill = await skills.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (skill == null)
                throw new HttpException((int) HttpStatusCode.NotFound, HttpMessages.SkillNotFound);

            //Delete skill from database
            _unitOfWork.Skills.Remove(skill);

            //Save changes in database
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResultViewModel<IList<Skill>>> SearchSkillsAsync(SearchSkillViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var skills = GetSkills(condition);
            var loadSkillResult = new SearchResultViewModel<IList<Skill>>();
            loadSkillResult.Total = await skills.CountAsync(cancellationToken);

            //do sorting
            skills = _dbService.Sort(skills, SortDirection.Ascending, UserDescriptionSortProperty.Id);

            // Do pagination.
            skills = _dbService.Paginate(skills, condition.Pagination);

            loadSkillResult.Records = await skills.ToListAsync(cancellationToken);
            return loadSkillResult;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Skill> SearchSkillAsync(SearchSkillViewModel condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetSkills(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Get skills using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<Skill> GetSkills(SearchSkillViewModel condition)
        {
            var skills = _unitOfWork.Skills.Search();

            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                var ids = condition.Ids.Where(c => c > 0).ToList();
                if (ids.Count > 0)
                    skills = skills.Where(c => ids.Contains(c.Id));
            }

            if (condition.Names != null && condition.Names.Count > 0)
            {
                var names = condition.Names.Where(c => !string.IsNullOrEmpty(c)).ToList();
                if (names.Count > 0)
                    skills = skills.Where(c => names.Any(name => c.Name.Contains(name)));
            }

            if (condition.CreatedTime != null)
            {
                var createdTime = condition.CreatedTime;
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    skills = skills.Where(skill => skill.CreatedTime >= from);

                if (to != null)
                    skills = skills.Where(skill => skill.CreatedTime <= to);
            }

            return skills;
        }
        #endregion
    }
}