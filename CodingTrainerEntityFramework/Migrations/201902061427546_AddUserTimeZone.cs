namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserTimeZone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "TimeZoneId", c => c.String(nullable: false, defaultValue: "GMT Standard Time"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "TimeZoneId");
        }
    }
}
