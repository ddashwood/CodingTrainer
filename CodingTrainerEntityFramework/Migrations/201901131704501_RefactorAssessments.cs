namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactorAssessments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssessmentsByInspection", "TokenSelector", c => c.String());
            AddColumn("dbo.AssessmentsByInspection", "ParentCondition", c => c.String());
            AddColumn("dbo.AssessmentsByInspection", "TokenSelector1", c => c.String());
            AddColumn("dbo.AssessmentsByInspection", "ParentCondition1", c => c.String());
            AddColumn("dbo.AssessmentsByInspection", "Condition", c => c.String());
            AddColumn("dbo.AssessmentsByRunning", "ExpectedResult1", c => c.String());
            AddColumn("dbo.AssessmentsByRunning", "Condition", c => c.String());
            AlterColumn("dbo.AssessmentsByRunning", "ExpectedResult", c => c.String());
            DropColumn("dbo.AssessmentsByInspection", "InspectionInstructions");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssessmentsByInspection", "InspectionInstructions", c => c.String(nullable: false));
            AlterColumn("dbo.AssessmentsByRunning", "ExpectedResult", c => c.String(nullable: false));
            DropColumn("dbo.AssessmentsByRunning", "Condition");
            DropColumn("dbo.AssessmentsByRunning", "ExpectedResult1");
            DropColumn("dbo.AssessmentsByInspection", "Condition");
            DropColumn("dbo.AssessmentsByInspection", "ParentCondition1");
            DropColumn("dbo.AssessmentsByInspection", "TokenSelector1");
            DropColumn("dbo.AssessmentsByInspection", "ParentCondition");
            DropColumn("dbo.AssessmentsByInspection", "TokenSelector");
        }
    }
}
