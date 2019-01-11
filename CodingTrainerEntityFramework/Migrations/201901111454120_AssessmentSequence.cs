namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssessmentSequence : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Assessments", new[] { "ChapterNo", "ExerciseNo" });
            AddColumn("dbo.Assessments", "Sequence", c => c.Int(nullable: false));
            CreateIndex("dbo.Assessments", new[] { "ChapterNo", "ExerciseNo", "Sequence" }, unique: true, name: "IX_Assessment_Sequence");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Assessments", "IX_Assessment_Sequence");
            DropColumn("dbo.Assessments", "Sequence");
            CreateIndex("dbo.Assessments", new[] { "ChapterNo", "ExerciseNo" });
        }
    }
}
