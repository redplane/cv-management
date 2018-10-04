using System;
using System.Data;
using System.Threading.Tasks;
using DbEntity.Interfaces;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DbEntity.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Properties
        
        /// <summary>
        ///     Provide methods to access confession database.
        /// </summary>
        private readonly DbContext _dbContext;

        public IRepository<Hobby> Hobbies { get; }
        public IRepository<Project> Projects { get; }
        public IRepository<ProjectResponsibility> ProjectResponsibilities { get; }
        public IRepository<ProjectSkill> ProjectSkills { get; }
        public IRepository<Responsibility> Responsibilities { get; }
        public IRepository<Skill> Skills { get; }
        public IRepository<SkillCategory> SkillCategories { get; }
        public IRepository<SkillCategorySkillRelationship> SkillCategorySkillRelationships { get; }
        public IRepository<User> Users { get; }
        public IRepository<UserDescription> UserDescriptions { get; }
        public IRepository<ProfileActivationToken> ProfileActivationTokens { get; }

        #endregion

        #region Constructors

        public UnitOfWork(IRepository<Hobby> hobbies,
            IRepository<Project> projects,
            IRepository<ProjectResponsibility> projectResponsibilities,
            IRepository<ProjectSkill> projectSkills,
            IRepository<Responsibility> responsibilities,
            IRepository<Skill> skills,
            IRepository<SkillCategory> skillCategories,
            IRepository<SkillCategorySkillRelationship> skillCategorySkillRelationships,
            IRepository<User> users,
            IRepository<UserDescription> userDescriptions, 
            IRepository<ProfileActivationToken> profileActivationTokens, 
            DbContext dbContext)
        {
            Hobbies = hobbies;
            Projects = projects;
            ProjectResponsibilities = projectResponsibilities;
            ProjectSkills = projectSkills;
            Responsibilities = responsibilities;
            Skills = skills;
            SkillCategories = skillCategories;
            SkillCategorySkillRelationships = skillCategorySkillRelationships;
            Users = users;
            UserDescriptions = userDescriptions;
            ProfileActivationTokens = profileActivationTokens;
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Save changes into database.
        /// </summary>
        /// <returns></returns>
        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        /// <summary>
        ///     Save changes into database asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Begin transaction scope.
        /// </summary>
        /// <returns></returns>
        public IDbContextTransaction BeginTransactionScope()
        {
            return _dbContext.Database.BeginTransaction();
        }

        /// <summary>
        /// Begin transaction scope.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public IDbContextTransaction BeginTransactionScope(IsolationLevel isolationLevel)
        {
            return _dbContext.Database.BeginTransaction(isolationLevel);
        }
        
        #endregion
    }
}