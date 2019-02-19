namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssessmentGroup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assessments", new[] { "ChapterNo", "ExerciseNo" }, "dbo.Exercises");
            DropIndex("dbo.Assessments", "IX_Assessment_Sequence");
            CreateTable(
                "dbo.AssessmentGroups",
                c => new
                    {
                        ChapterNo = c.Int(nullable: false),
                        ExerciseNo = c.Int(nullable: false),
                        AssessmentGroupId = c.Int(nullable: false, identity: true),
                        Sequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssessmentGroupId)
                .ForeignKey("dbo.Exercises", t => new { t.ChapterNo, t.ExerciseNo }, cascadeDelete: true)
                .Index(t => new { t.ChapterNo, t.ExerciseNo, t.Sequence }, unique: true, name: "IX_AssessmentGroup_Sequence");
            
            AddColumn("dbo.Assessments", "AssessmentGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Assessments", new[] { "AssessmentGroupId", "Sequence" }, unique: true, name: "IX_Assessment_Sequence");
            AddForeignKey("dbo.Assessments", "AssessmentGroupId", "dbo.AssessmentGroups", "AssessmentGroupId", cascadeDelete: true);
            DropColumn("dbo.Assessments", "ChapterNo");
            DropColumn("dbo.Assessments", "ExerciseNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assessments", "ExerciseNo", c => c.Int(nullable: false));
            AddColumn("dbo.Assessments", "ChapterNo", c => c.Int(nullable: false));
            DropForeignKey("dbo.AssessmentGroups", new[] { "ChapterNo", "ExerciseNo" }, "dbo.Exercises");
            DropForeignKey("dbo.Assessments", "AssessmentGroupId", "dbo.AssessmentGroups");
            DropIndex("dbo.Assessments", "IX_Assessment_Sequence");
            DropIndex("dbo.AssessmentGroups", "IX_AssessmentGroup_Sequence");
            DropColumn("dbo.Assessments", "AssessmentGroupId");
            DropTable("dbo.AssessmentGroups");
            CreateIndex("dbo.Assessments", new[] { "ChapterNo", "ExerciseNo", "Sequence" }, unique: true, name: "IX_Assessment_Sequence");
            AddForeignKey("dbo.Assessments", new[] { "ChapterNo", "ExerciseNo" }, "dbo.Exercises", new[] { "ChapterNo", "ExerciseNo" }, cascadeDelete: true);
        }
    }
}
