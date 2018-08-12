namespace DbEntity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitalDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hobby",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Password = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Photo = c.String(),
                        Birthday = c.Double(nullable: false),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Project",
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
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ProjectResponsibility",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        ResponsibilityId = c.Int(nullable: false),
                        CreatedTime = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.ResponsibilityId })
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.Responsibility", t => t.ResponsibilityId)
                .Index(t => t.ProjectId)
                .Index(t => t.ResponsibilityId);
            
            CreateTable(
                "dbo.Responsibility",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedTime = c.Double(nullable: false),
                        LastModifiedTime = c.Double(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectSkill",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.SkillId })
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.Skill", t => t.SkillId)
                .Index(t => t.ProjectId)
                .Index(t => t.SkillId);
            
            CreateTable(
                "dbo.Skill",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedTime = c.Double(nullable: false),
                        LastModifiedTime = c.Double(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SkillCategorySkillRelationship",
                c => new
                    {
                        SkillCategoryId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                        Point = c.Int(nullable: false),
                        CreatedTime = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.SkillCategoryId, t.SkillId })
                .ForeignKey("dbo.Skill", t => t.SkillId)
                .ForeignKey("dbo.SkillCategory", t => t.SkillCategoryId)
                .Index(t => t.SkillCategoryId)
                .Index(t => t.SkillId);
            
            CreateTable(
                "dbo.SkillCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Photo = c.String(),
                        Name = c.String(),
                        CreatedTime = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserDescription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hobby", "UserId", "dbo.User");
            DropForeignKey("dbo.UserDescription", "UserId", "dbo.User");
            DropForeignKey("dbo.Project", "UserId", "dbo.User");
            DropForeignKey("dbo.ProjectSkill", "SkillId", "dbo.Skill");
            DropForeignKey("dbo.SkillCategorySkillRelationship", "SkillCategoryId", "dbo.SkillCategory");
            DropForeignKey("dbo.SkillCategory", "UserId", "dbo.User");
            DropForeignKey("dbo.SkillCategorySkillRelationship", "SkillId", "dbo.Skill");
            DropForeignKey("dbo.ProjectSkill", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.ProjectResponsibility", "ResponsibilityId", "dbo.Responsibility");
            DropForeignKey("dbo.ProjectResponsibility", "ProjectId", "dbo.Project");
            DropIndex("dbo.UserDescription", new[] { "UserId" });
            DropIndex("dbo.SkillCategory", new[] { "UserId" });
            DropIndex("dbo.SkillCategorySkillRelationship", new[] { "SkillId" });
            DropIndex("dbo.SkillCategorySkillRelationship", new[] { "SkillCategoryId" });
            DropIndex("dbo.ProjectSkill", new[] { "SkillId" });
            DropIndex("dbo.ProjectSkill", new[] { "ProjectId" });
            DropIndex("dbo.ProjectResponsibility", new[] { "ResponsibilityId" });
            DropIndex("dbo.ProjectResponsibility", new[] { "ProjectId" });
            DropIndex("dbo.Project", new[] { "UserId" });
            DropIndex("dbo.Hobby", new[] { "UserId" });
            DropTable("dbo.UserDescription");
            DropTable("dbo.SkillCategory");
            DropTable("dbo.SkillCategorySkillRelationship");
            DropTable("dbo.Skill");
            DropTable("dbo.ProjectSkill");
            DropTable("dbo.Responsibility");
            DropTable("dbo.ProjectResponsibility");
            DropTable("dbo.Project");
            DropTable("dbo.User");
            DropTable("dbo.Hobby");
        }
    }
}
