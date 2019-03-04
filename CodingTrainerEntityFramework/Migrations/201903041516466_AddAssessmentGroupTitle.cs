namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssessmentGroupTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssessmentGroups", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssessmentGroups", "Title");
        }
    }
}
