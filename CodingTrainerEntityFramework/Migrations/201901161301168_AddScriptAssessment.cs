namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScriptAssessment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssessmentsByInspection", "Script", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssessmentsByInspection", "Script");
        }
    }
}
