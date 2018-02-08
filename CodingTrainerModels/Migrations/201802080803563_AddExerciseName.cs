namespace CodingTrainer.CodingTrainerModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExerciseName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exercises", "ExerciseName", c => c.String(nullable: false, defaultValue: "Exercise Name"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exercises", "ExerciseName");
        }
    }
}
