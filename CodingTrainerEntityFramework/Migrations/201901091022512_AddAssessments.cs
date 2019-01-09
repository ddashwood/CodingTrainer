namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssessments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assessment",
                c => new
                    {
                        AssessmentId = c.Int(nullable: false, identity: true),
                        ChapterNo = c.Int(nullable: false),
                        ExerciseNo = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AssessmentId)
                .ForeignKey("dbo.Exercises", t => new { t.ChapterNo, t.ExerciseNo })
                .Index(t => new { t.ChapterNo, t.ExerciseNo });
            
            CreateTable(
                "dbo.AssessmentByInspection",
                c => new
                    {
                        AssessmentId = c.Int(nullable: false),
                        InspectionInstructions = c.String(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AssessmentId)
                .ForeignKey("dbo.Assessment", t => t.AssessmentId)
                .Index(t => t.AssessmentId);
            
            CreateTable(
                "dbo.AssessmentByRunning",
                c => new
                    {
                        AssessmentId = c.Int(nullable: false),
                        ConsoleInText = c.String(nullable: false),
                        ExpectedResult = c.String(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AssessmentId)
                .ForeignKey("dbo.Assessment", t => t.AssessmentId)
                .Index(t => t.AssessmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssessmentByRunning", "AssessmentId", "dbo.Assessment");
            DropForeignKey("dbo.AssessmentByInspection", "AssessmentId", "dbo.Assessment");
            DropForeignKey("dbo.Assessment", new[] { "ChapterNo", "ExerciseNo" }, "dbo.Exercises");
            DropIndex("dbo.AssessmentByRunning", new[] { "AssessmentId" });
            DropIndex("dbo.AssessmentByInspection", new[] { "AssessmentId" });
            DropIndex("dbo.Assessment", new[] { "ChapterNo", "ExerciseNo" });
            DropTable("dbo.AssessmentByRunning");
            DropTable("dbo.AssessmentByInspection");
            DropTable("dbo.Assessment");
        }
    }
}
