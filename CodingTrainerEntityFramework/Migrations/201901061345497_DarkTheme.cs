namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DarkTheme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Dark", c => c.Boolean(nullable: false, defaultValue: false));
            DropColumn("dbo.AspNetUsers", "SelectedTheme");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "SelectedTheme", c => c.String(nullable: false));
            DropColumn("dbo.AspNetUsers", "Dark");
        }
    }
}
