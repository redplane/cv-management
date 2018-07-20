namespace Cv_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonalSkills",
                c => new
                    {
                        SkillCategoryId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                        Point = c.Int(nullable: false),
                        CreatedTime = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.SkillCategoryId, t.SkillId })
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .ForeignKey("dbo.SkillCategories", t => t.SkillCategoryId, cascadeDelete: true)
                .Index(t => t.SkillCategoryId)
                .Index(t => t.SkillId);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedTime = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectSkills",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.SkillId })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.SkillId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        StatedTime = c.Double(nullable: false),
                        FinishedTime = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ProjectResponsibilities",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        RespinsibilityId = c.Int(nullable: false),
                        CreatedTime = c.Double(nullable: false),
                        Responsibility_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.RespinsibilityId })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.Responsibilities", t => t.Responsibility_Id)
                .Index(t => t.ProjectId)
                .Index(t => t.Responsibility_Id);
            
            CreateTable(
                "dbo.Responsibilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedTime = c.Double(nullable: false),
                        LastModifiedTime = c.Double(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Photo = c.String(),
                        Birthday = c.Double(nullable: false),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SkillCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Photo = c.String(),
                        Name = c.String(),
                        CreatedTime = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserDescriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectSkills", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.UserDescriptions", "UserId", "dbo.Users");
            DropForeignKey("dbo.SkillCategories", "UserId", "dbo.Users");
            DropForeignKey("dbo.PersonalSkills", "SkillCategoryId", "dbo.SkillCategories");
            DropForeignKey("dbo.Projects", "UserId", "dbo.Users");
            DropForeignKey("dbo.ProjectSkills", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectResponsibilities", "Responsibility_Id", "dbo.Responsibilities");
            DropForeignKey("dbo.ProjectResponsibilities", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.PersonalSkills", "SkillId", "dbo.Skills");
            DropIndex("dbo.UserDescriptions", new[] { "UserId" });
            DropIndex("dbo.SkillCategories", new[] { "UserId" });
            DropIndex("dbo.ProjectResponsibilities", new[] { "Responsibility_Id" });
            DropIndex("dbo.ProjectResponsibilities", new[] { "ProjectId" });
            DropIndex("dbo.Projects", new[] { "UserId" });
            DropIndex("dbo.ProjectSkills", new[] { "SkillId" });
            DropIndex("dbo.ProjectSkills", new[] { "ProjectId" });
            DropIndex("dbo.PersonalSkills", new[] { "SkillId" });
            DropIndex("dbo.PersonalSkills", new[] { "SkillCategoryId" });
            DropTable("dbo.UserDescriptions");
            DropTable("dbo.SkillCategories");
            DropTable("dbo.Users");
            DropTable("dbo.Responsibilities");
            DropTable("dbo.ProjectResponsibilities");
            DropTable("dbo.Projects");
            DropTable("dbo.ProjectSkills");
            DropTable("dbo.Skills");
            DropTable("dbo.PersonalSkills");
        }
    }
}
