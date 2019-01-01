namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeOnDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExceptionLogs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Submissions", "UserId", "dbo.AspNetUsers");
            AddForeignKey("dbo.ExceptionLogs", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Submissions", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Submissions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExceptionLogs", "UserId", "dbo.AspNetUsers");
            AddForeignKey("dbo.Submissions", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ExceptionLogs", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
