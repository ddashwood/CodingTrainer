namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubmissionDetails : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Submissions", name: "Exercise_ChapterNo", newName: "ChapterNo");
            RenameColumn(table: "dbo.Submissions", name: "Exercise_ExerciseNo", newName: "ExerciseNo");
            RenameIndex(table: "dbo.Submissions", name: "IX_Exercise_ChapterNo_Exercise_ExerciseNo", newName: "IX_ChapterNo_ExerciseNo");
            AddColumn("dbo.Submissions", "SubmissionDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Submissions", "Output", c => c.String(nullable: false));
            AddColumn("dbo.Submissions", "Success", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Submissions", "Success");
            DropColumn("dbo.Submissions", "Output");
            DropColumn("dbo.Submissions", "SubmissionDateTime");
            RenameIndex(table: "dbo.Submissions", name: "IX_ChapterNo_ExerciseNo", newName: "IX_Exercise_ChapterNo_Exercise_ExerciseNo");
            RenameColumn(table: "dbo.Submissions", name: "ExerciseNo", newName: "Exercise_ExerciseNo");
            RenameColumn(table: "dbo.Submissions", name: "ChapterNo", newName: "Exercise_ChapterNo");
        }
    }
}
