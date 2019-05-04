namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUseResultFromPreviousAssessment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssessmentsByRunning", "UseResultFromPreviousAssessment", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssessmentsByRunning", "UseResultFromPreviousAssessment");
        }
    }
}
