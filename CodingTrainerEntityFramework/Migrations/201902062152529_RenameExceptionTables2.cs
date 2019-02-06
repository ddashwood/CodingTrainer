namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameExceptionTables2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ExceptionInUsersCodeLogs", newName: "ExceptionsRunningUsersCode");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ExceptionsRunningUsersCode", newName: "ExceptionInUsersCodeLogs");
        }
    }
}
