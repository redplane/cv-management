namespace DbEntity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Status");
        }
    }
}
