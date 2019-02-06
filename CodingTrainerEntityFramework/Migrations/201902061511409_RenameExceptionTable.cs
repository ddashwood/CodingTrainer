namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameExceptionTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ExceptionLogs", newName: "ExceptionInUsersCodeLogs");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ExceptionInUsersCodeLogs", newName: "ExceptionLogs");
        }
    }
}
