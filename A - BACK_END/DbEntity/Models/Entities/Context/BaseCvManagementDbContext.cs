using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DbEntity.Models.Entities.Context
{
    public class BaseCvManagementDbContext : DbContext
    {
        #region Constructors

        public BaseCvManagementDbContext(DbContextOptions<BaseCvManagementDbContext> options)
            : base(options)
        { }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when model is being created.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Initialize user table.
            InitializeUserTable(modelBuilder);

            // Initialize user description table.
            InitializeUserDescriptionTable(modelBuilder);

            // Initialize project table.
            InitializeProjectTable(modelBuilder);

            // Initialize project skill table.
            InitializeProjectSkillTable(modelBuilder);

            // Initialize skill table.
            InitializeSkillTable(modelBuilder);

            // Initialize project responsibility table.
            InitializeProjectResponsibilityTable(modelBuilder);

            // Initialize responsibility table.
            InitializeResponsibilityTable(modelBuilder);

            // Initialize skill table.
            InitializeSkillCategory(modelBuilder);

            // Initialize skill category skill relationship table.
            InitializeSkillCategorySkillRelationshipTable(modelBuilder);

            //Initialize hobby table
            InitializeHobbyTable(modelBuilder);

            // Initialize profile activation token
            InitializeProfileActivationToken(modelBuilder);

            // This is for remove pluralization naming convention in database defined by Entity Framework.
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                entity.Relational().TableName = entity.DisplayName();

            // Disable cascade delete.
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            
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
        public DbSet<SkillCategorySkillRelationship> SkillCategorySkillRelationships { get; set; }

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

        /// <summary>
        /// List of user hobbies
        /// </summary>
        public DbSet<Hobby> Hobbies { get; set; }

        /// <summary>
        /// List of profile activation tokens.
        /// </summary>
        public DbSet<ProfileActivationToken> ProfileActivationTokens { get; set; }

        #endregion

        #region Table initialization

        /// <summary>
        ///     Initialize user table.
        /// </summary>
        private void InitializeUserTable(ModelBuilder dbModelBuilder)
        {
            var user = dbModelBuilder.Entity<User>();
            user.HasKey(x => x.Id);
            user.Property(x => x.Id).UseSqlServerIdentityColumn();
        }

        /// <summary>
        /// Initialize user description tabe,
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeUserDescriptionTable(ModelBuilder dbModelBuilder)
        {
            var userDescription = dbModelBuilder.Entity<UserDescription>();
            userDescription.HasKey(x => x.Id);
            userDescription.Property(x => x.Id).UseSqlServerIdentityColumn();

            userDescription.HasOne(x => x.User).WithMany(x => x.UserDescriptions).HasForeignKey(x => x.UserId);
        }

        /// <summary>
        ///     Initialize project table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeProjectTable(ModelBuilder dbModelBuilder)
        {
            var project = dbModelBuilder.Entity<Project>();
            project.HasKey(x => x.Id);
            project.Property(x => x.Id).UseSqlServerIdentityColumn();

            project.HasOne(x => x.User).WithMany(x => x.Projects).HasForeignKey(x => x.UserId);
        }

        /// <summary>
        ///     Initialize project skill table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeProjectSkillTable(ModelBuilder dbModelBuilder)
        {
            var projectSkill = dbModelBuilder.Entity<ProjectSkill>();
            projectSkill.HasKey(x => new { x.ProjectId, x.SkillId });

            projectSkill.HasOne(x => x.Skill).WithMany(x => x.ProjectSkills).HasForeignKey(x => x.SkillId);
            projectSkill.HasOne(x => x.Project).WithMany(x => x.ProjectSkills).HasForeignKey(x => x.ProjectId);
        }

        /// <summary>
        ///     Initialize skill table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeSkillTable(ModelBuilder dbModelBuilder)
        {
            var skill = dbModelBuilder.Entity<Skill>();
            skill.HasKey(x => x.Id);
            skill.Property(x => x.Id).UseSqlServerIdentityColumn();
        }

        /// <summary>
        ///     Initialize project responsibility table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeProjectResponsibilityTable(ModelBuilder dbModelBuilder)
        {
            var projectResponsibility = dbModelBuilder.Entity<ProjectResponsibility>();
            projectResponsibility.HasKey(x => new { x.ProjectId, x.ResponsibilityId });

            projectResponsibility.HasOne(x => x.Project).WithMany(x => x.ProjectResponsibilities)
                .HasForeignKey(x => x.ProjectId);

            projectResponsibility.HasOne(x => x.Responsibility).WithMany(x => x.ProjectResponsibilities)
                .HasForeignKey(x => x.ResponsibilityId);
        }

        /// <summary>
        ///     Initialize responsibility table.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeResponsibilityTable(ModelBuilder dbModelBuilder)
        {
            var responsibility = dbModelBuilder.Entity<Responsibility>();
            responsibility.HasKey(x => x.Id);
        }

        /// <summary>
        ///     Initialize skill category.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeSkillCategory(ModelBuilder dbModelBuilder)
        {
            var skillCategory = dbModelBuilder.Entity<SkillCategory>();
            skillCategory.HasKey(x => x.Id);
            skillCategory.Property(x => x.Id).UseSqlServerIdentityColumn();

            skillCategory.HasOne(x => x.User).WithMany(x => x.SkillCategories).HasForeignKey(x => x.UserId);
        }

        /// <summary>
        ///     Initialize skill category - skill relationship.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeSkillCategorySkillRelationshipTable(ModelBuilder dbModelBuilder)
        {
            var skillCategorySkillRelationship = dbModelBuilder.Entity<SkillCategorySkillRelationship>();
            skillCategorySkillRelationship.HasKey(x => new { x.SkillCategoryId, x.SkillId });

            skillCategorySkillRelationship.HasOne(x => x.Skill).WithMany(x => x.SkillCategorySkillRelationships).HasForeignKey(x => x.SkillId);
            skillCategorySkillRelationship.HasOne(x => x.SkillCategory)
                .WithMany(x => x.SkillCategorySkillRelationships).HasForeignKey(x => x.SkillCategoryId);
        }

        /// <summary>
        /// Initialize hobby
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        private void InitializeHobbyTable(ModelBuilder dbModelBuilder)
        {
            var hobby = dbModelBuilder.Entity<Hobby>();
            hobby.HasKey(x => x.Id);
            hobby.Property(x => x.Id).UseSqlServerIdentityColumn();
            hobby.HasOne(x => x.User).WithMany(x => x.Hobbies).HasForeignKey(x => x.UserId);

        }

        /// <summary>
        /// Initialize profile activation token.
        /// </summary>
        /// <param name="dbModelBuilder"></param>
        public void InitializeProfileActivationToken(ModelBuilder dbModelBuilder)
        {
            var profileActivationToken = dbModelBuilder.Entity<ProfileActivationToken>();
            profileActivationToken.HasKey(x => x.Email);
            profileActivationToken.Property(x => x.Email).IsRequired();
        }
        #endregion
    }
}