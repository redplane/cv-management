namespace Cv_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastModifiedTimeColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Skills", "LastModifiedTime", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Skills", "LastModifiedTime");
        }
    }
}
