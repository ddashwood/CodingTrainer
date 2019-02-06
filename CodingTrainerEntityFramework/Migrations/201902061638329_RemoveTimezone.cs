namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTimezone : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "TimeZoneId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "TimeZoneId", c => c.String(nullable: false));
        }
    }
}
