namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExerciseContent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exercises", "Content", c => c.String(nullable: false, defaultValue:"<p>Placeholder for course content</p>"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exercises", "Content");
        }
    }
}
