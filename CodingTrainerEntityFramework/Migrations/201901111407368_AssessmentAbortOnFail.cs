namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssessmentAbortOnFail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assessments", "AbortOnFail", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assessments", "AbortOnFail");
        }
    }
}
