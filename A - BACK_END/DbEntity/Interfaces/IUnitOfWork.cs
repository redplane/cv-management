using System.Data;
using System.Threading.Tasks;
using DbEntity.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace DbEntity.Interfaces
{
    public interface IUnitOfWork
    {
        #region Properties

        IRepository<Hobby> Hobbies { get; }

        IRepository<Project> Projects { get; }

        IRepository<ProjectResponsibility> ProjectResponsibilities { get; }

        IRepository<ProjectSkill> ProjectSkills { get; }

        IRepository<Responsibility> Responsibilities { get; }

        IRepository<Skill> Skills { get; }

        IRepository<SkillCategory> SkillCategories { get; }

        IRepository<SkillCategorySkillRelationship> SkillCategorySkillRelationships { get; }

        IRepository<User> Users { get; }

        IRepository<UserDescription> UserDescriptions { get; }

        IRepository<ProfileActivationToken> ProfileActivationTokens { get; }

        #endregion

        #region Methods

        #region Methods

        /// <summary>
        ///     Save changes into database.
        /// </summary>
        /// <returns></returns>
        int Commit();

        /// <summary>
        ///     Save changes into database asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync();

        /// <summary>
        /// Begin transaction scope.
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction BeginTransactionScope();

        /// <summary>
        /// Begin transaction scope.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        IDbContextTransaction BeginTransactionScope(IsolationLevel isolationLevel);

        #endregion

        #endregion
    }
}