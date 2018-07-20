namespace Cv_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addHobbyTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hobbies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Users", "Email", c => c.String());
            AddColumn("dbo.Users", "Password", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hobbies", "UserId", "dbo.Users");
            DropIndex("dbo.Hobbies", new[] { "UserId" });
            DropColumn("dbo.Users", "Password");
            DropColumn("dbo.Users", "Email");
            DropTable("dbo.Hobbies");
        }
    }
}
