namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropUniData : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "AssessByRunningOnly");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "AssessByRunningOnly", c => c.Boolean(nullable: false));
        }
    }
}
