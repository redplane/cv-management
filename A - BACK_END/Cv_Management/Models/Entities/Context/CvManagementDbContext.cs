using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Cv_Management.Models.Entities.Context
{
    public class CvManagementDbContext : DbContext
    {
        #region Constructors

        public CvManagementDbContext() : base("CvManagement")
        {
            Database.SetInitializer<CvManagementDbContext>(null);
        }

        #endregion

        #region Overriden methods

        /// <summary>
        ///     Called when model is being created.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder dbModelBuilder)
        {
            // Remove pluralizing naming convention.
            dbModelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Prevent database from cascade deleting.
            dbModelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Initialize user table.
            InitializeUserTable(dbModelBuilder);

            // Initialize user description table.
            InitializeUserDescriptionTable(dbModelBuilder);

            // Initialize project table.
            InitializeProjectTable(dbModelBuilder);

            // Initialize project skill table.
            InitializeProjectSkillTable(dbModelBuilder);

            // Initialize skill table.
            InitializeSkillTable(dbModelBuilder);

            // Initialize project responsibility table.
            InitializeProjectResponsibilityTable(dbModelBuilder);

            // Initialize responsibility table.
            InitializeResponsibilityTable(dbModelBuilder);

            // Initialize skill table.
            InitializeSkillCategory(dbModelBuilder);

            // Initialize skill category skill relationship table.
            InitializeSkillCategorySkillRelationshipTable(dbModelBuilder);

            base.OnModelCreating(dbModelBuilder);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     List of projects.
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        ///     List of skills.
        /// </summary>
        public DbSet<Skill> Skills { get; set; }

        /// <summary>
        ///     List of personal skills.
        /// </summary>
        public DbSet<SkillCategorySkillRelationship> PersonalSkills { get; set; }

        /// <summary>
        ///     List of project responsibilities.
        /// </summary>
        public DbSet<ProjectResponsibility> ProjectResponsibilities { get; set; }

        /// <summary>
        ///     List of project skills.
        /// </summary>
        public DbSet<ProjectSkill> ProjectSkills { get; set; }

        /// <summary>
        ///     List of responsibilities.
        /// </summary>
        public DbSet<Responsibility> Responsibilities { get; set; }

        /// <summary>
        ///     List of skill categories.
        /// </summary>
        public DbSet<SkillCategory> SkillCategories { get; set; }

        /// <summary>
        ///     List of users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        ///     List of user descriptions.
        /// </summary>
        public DbSet<UserDescription> UserDescriptions { get; set; }

        #endregion

        #region Table initialization

        /// <summary>
        ///     Initialize user table.
        /// </summary>
        private void InitializeUserTable(DbModelBuilder dbModelBuilder)
        {
            var user = dbModelBuilder.Entity<User>();
            user.HasKey(x => x.Id);
            user.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        /// <summary>
        /// Initialize user description tabe,
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeUserDescriptionTable(DbModelBuilder dbModelBuilder)
        {
            var userDescription = dbModelBuilder.Entity<UserDescription>();
            userDescription.HasKey(x => x.Id);
            userDescription.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            userDescription.HasRequired(x => x.User).WithMany(x => x.UserDescriptions).HasForeignKey(x => x.UserId);
        }

        /// <summary>
        ///     Initialize project table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeProjectTable(DbModelBuilder dbModelBuilder)
        {
            var project = dbModelBuilder.Entity<Project>();
            project.HasKey(x => x.Id);
            project.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            project.HasRequired(x => x.User).WithMany(x => x.Projects).HasForeignKey(x => x.UserId);
        }

        /// <summary>
        ///     Initialize project skill table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeProjectSkillTable(DbModelBuilder dbModelBuilder)
        {
            var projectSkill = dbModelBuilder.Entity<ProjectSkill>();
            projectSkill.HasKey(x => new { x.ProjectId, x.SkillId });

            projectSkill.HasRequired(x => x.Skill).WithMany(x => x.ProjectSkills).HasForeignKey(x => x.SkillId);
            projectSkill.HasRequired(x => x.Project).WithMany(x => x.ProjectSkills).HasForeignKey(x => x.ProjectId);
        }

        /// <summary>
        ///     Initialize skill table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeSkillTable(DbModelBuilder dbModelBuilder)
        {
            var skill = dbModelBuilder.Entity<Skill>();
            skill.HasKey(x => x.Id);
            skill.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        /// <summary>
        ///     Initialize project responsibility table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeProjectResponsibilityTable(DbModelBuilder dbModelBuilder)
        {
            var projectResponsibility = dbModelBuilder.Entity<ProjectResponsibility>();
            projectResponsibility.HasKey(x => new { x.ProjectId, x.ResponsibilityId });

            projectResponsibility.HasRequired(x => x.Project).WithMany(x => x.ProjectResponsibilities)
                .HasForeignKey(x => x.ResponsibilityId);

            projectResponsibility.HasRequired(x => x.Responsibility).WithMany(x => x.ProjectResponsibilities)
                .HasForeignKey(x => x.ResponsibilityId);
        }

        /// <summary>
        ///     Initialize responsibility table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeResponsibilityTable(DbModelBuilder dbModelBuilder)
        {
            var responsibility = dbModelBuilder.Entity<Responsibility>();
            responsibility.HasKey(x => x.Id);
        }

        /// <summary>
        ///     Initialize skill category.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeSkillCategory(DbModelBuilder dbModelBuilder)
        {
            var skillCategory = dbModelBuilder.Entity<SkillCategory>();
            skillCategory.HasKey(x => x.Id);
            skillCategory.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            skillCategory.HasRequired(x => x.User).WithMany(x => x.SkillCategories).HasForeignKey(x => x.UserId);
        }

        /// <summary>
        ///     Initialize skill category - skill relationship.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeSkillCategorySkillRelationshipTable(DbModelBuilder dbModelBuilder)
        {
            var skillCategorySkillRelationship = dbModelBuilder.Entity<SkillCategorySkillRelationship>();
            skillCategorySkillRelationship.HasKey(x => new { x.SkillCategoryId, x.SkillId });

            skillCategorySkillRelationship.HasRequired(x => x.Skill).WithMany(x => x.SkillCategorySkillRelationships).HasForeignKey(x => x.SkillId);
            skillCategorySkillRelationship.HasRequired(x => x.SkillCategory)
                .WithMany(x => x.SkillCategorySkillRelationships).HasForeignKey(x => x.SkillCategoryId);
        }

        #endregion
    }
}