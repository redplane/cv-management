using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace Cv_Management.Entities.Context
{
    public class CvManagementDbContext : DbContext
    {
        #region Constructors

        public CvManagementDbContext() : base("DefaultConnection") { }

        #endregion

        #region Properties

        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<PersonalSkill> PersonalSkills { get; set; }
        public DbSet<ProjectResponsibility> ProjectResponsibilities { get; set; }
        public DbSet<ProjectSkill> ProjectSkills { get; set; }
        public DbSet<Responsibility> Responsibilities { get; set; }
        public DbSet<SkillCategory> SkillCategories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDescription> UserDescriptions { get; set; }

        #endregion
    }
}