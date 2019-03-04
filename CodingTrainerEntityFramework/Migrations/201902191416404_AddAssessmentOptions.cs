namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssessmentOptions : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Assessments", "AbortOnFail", "EndAssessmentGroupOnFail");
            AddColumn("dbo.AssessmentGroups", "ShowAutoMessageOnStart", c => c.Boolean(nullable: false, defaultValue:true));
            AddColumn("dbo.AssessmentGroups", "ShowAutoMessageOnPass", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.AssessmentGroups", "ShowAutoMessageOnFail", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.AssessmentGroups", "EndAssessmentsOnFail", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Assessments", "ShowAutoMessageOnStart", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Assessments", "ShowAutoMessageOnPass", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Assessments", "ShowAutoMessageOnFail", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.AssessmentsByRunning", "ShowScriptRunning", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssessmentsByRunning", "ShowScriptRunning");
            DropColumn("dbo.Assessments", "ShowAutoMessageOnFail");
            DropColumn("dbo.Assessments", "ShowAutoMessageOnPass");
            DropColumn("dbo.Assessments", "ShowAutoMessageOnStart");
            DropColumn("dbo.AssessmentGroups", "EndAssessmentsOnFail");
            DropColumn("dbo.AssessmentGroups", "ShowAutoMessageOnFail");
            DropColumn("dbo.AssessmentGroups", "ShowAutoMessageOnPass");
            DropColumn("dbo.AssessmentGroups", "ShowAutoMessageOnStart");
            RenameColumn("dbo.Assessments", "EndAssessmentGroupOnFail", "AbortOnFail");

        }
    }
}
