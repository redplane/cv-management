namespace DbEntity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfileActivationToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProfileActivationToken",
                c => new
                    {
                        Email = c.String(nullable: false, maxLength: 128),
                        Token = c.String(),
                        CreatedTime = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Email);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProfileActivationToken");
        }
    }
}
