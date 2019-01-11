namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssessmentsCascadeDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AssessmentsByRunning", "AssessmentId", "dbo.Assessments");
            DropForeignKey("dbo.AssessmentsByInspection", "AssessmentId", "dbo.Assessments");
            DropForeignKey("dbo.Assessments", new[] { "ChapterNo", "ExerciseNo" }, "dbo.Exercises");
            DropIndex("dbo.AssessmentsByRunning", new[] { "AssessmentId" });
            DropIndex("dbo.AssessmentsByInspection", new[] { "AssessmentId" });
            DropIndex("dbo.Assessments", new[] { "ChapterNo", "ExerciseNo" });
            DropTable("dbo.AssessmentsByRunning");
            DropTable("dbo.AssessmentsByInspection");
            DropTable("dbo.Assessments");

                        CreateTable(
                "dbo.Assessments",
                c => new
                    {
                        AssessmentId = c.Int(nullable: false, identity: true),
                        ChapterNo = c.Int(nullable: false),
                        ExerciseNo = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AssessmentId)
                .ForeignKey("dbo.Exercises", t => new { t.ChapterNo, t.ExerciseNo }, cascadeDelete: true)
                .Index(t => new { t.ChapterNo, t.ExerciseNo });
            
            CreateTable(
                "dbo.AssessmentsByInspection",
                c => new
                    {
                        AssessmentId = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        InspectionInstructions = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AssessmentId)
                .ForeignKey("dbo.Assessments", t => t.AssessmentId, cascadeDelete: true)
                .Index(t => t.AssessmentId);
            
            CreateTable(
                "dbo.AssessmentsByRunning",
                c => new
                    {
                        AssessmentId = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        ConsoleInText = c.String(nullable: false),
                        ExpectedResult = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AssessmentId)
                .ForeignKey("dbo.Assessments", t => t.AssessmentId, cascadeDelete: true)
                .Index(t => t.AssessmentId);
            

        }
        
        public override void Down()
        {
        }
    }
}
