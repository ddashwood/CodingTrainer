namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssessmentMessagesEtc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assessments", "EndAssessmentGroupOnPass", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Assessments", "StartMessage", c => c.String());
            AddColumn("dbo.Assessments", "PassMessage", c => c.String());
            AddColumn("dbo.Assessments", "FailMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assessments", "FailMessage");
            DropColumn("dbo.Assessments", "PassMessage");
            DropColumn("dbo.Assessments", "StartMessage");
            DropColumn("dbo.Assessments", "EndAssessmentGroupOnPass");
        }
    }
}
