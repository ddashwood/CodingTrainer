namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSavedWork : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SavedWork",
                c => new
                    {
                        SavedWorkId = c.Int(nullable: false, identity: true),
                        ChapterNo = c.Int(nullable: false),
                        ExerciseNo = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        SavedCode = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.SavedWorkId)
                .ForeignKey("dbo.Exercises", t => new { t.ChapterNo, t.ExerciseNo }, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => new { t.UserId, t.ChapterNo, t.ExerciseNo }, unique: true, name: "IX_SavedWorkUserExercise");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavedWork", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SavedWork", new[] { "ChapterNo", "ExerciseNo" }, "dbo.Exercises");
            DropIndex("dbo.SavedWork", "IX_SavedWorkUserExercise");
            DropTable("dbo.SavedWork");
        }
    }
}
