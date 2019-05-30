namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExerciseFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exercises", "ModelAnswer", c => c.String());
            AddColumn("dbo.Exercises", "IsAssessed", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Exercises", "IsFinalExercise", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exercises", "IsFinalExercise");
            DropColumn("dbo.Exercises", "IsAssessed");
            DropColumn("dbo.Exercises", "ModelAnswer");
        }
    }
}
