namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrentExercise : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CurrentChapterNo", c => c.Int(nullable: false, defaultValue: 1));
            AddColumn("dbo.AspNetUsers", "CurrentExerciseNo", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.AspNetUsers", new[] { "CurrentChapterNo", "CurrentExerciseNo" });
            AddForeignKey("dbo.AspNetUsers", new[] { "CurrentChapterNo", "CurrentExerciseNo" }, "dbo.Exercises", new[] { "ChapterNo", "ExerciseNo" }, cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", new[] { "CurrentChapterNo", "CurrentExerciseNo" }, "dbo.Exercises");
            DropIndex("dbo.AspNetUsers", new[] { "CurrentChapterNo", "CurrentExerciseNo" });
            DropColumn("dbo.AspNetUsers", "CurrentExerciseNo");
            DropColumn("dbo.AspNetUsers", "CurrentChapterNo");
        }
    }
}
