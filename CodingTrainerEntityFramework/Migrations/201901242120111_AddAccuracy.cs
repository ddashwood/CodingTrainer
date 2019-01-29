namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccuracy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssessmentsByRunning", "RequiredAccuracy", c => c.Double());
            AddColumn("dbo.AssessmentsByRunning", "RequiredAccuracy1", c => c.Double());
            DropColumn("dbo.AssessmentsByInspection", "TokenSelector1");
            DropColumn("dbo.AssessmentsByInspection", "ParentCondition1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssessmentsByInspection", "ParentCondition1", c => c.String());
            AddColumn("dbo.AssessmentsByInspection", "TokenSelector1", c => c.String());
            DropColumn("dbo.AssessmentsByRunning", "RequiredAccuracy1");
            DropColumn("dbo.AssessmentsByRunning", "RequiredAccuracy");
        }
    }
}
