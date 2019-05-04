namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsoleInTextNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AssessmentsByRunning", "ConsoleInText", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AssessmentsByRunning", "ConsoleInText", c => c.String(nullable: false));
        }
    }
}
