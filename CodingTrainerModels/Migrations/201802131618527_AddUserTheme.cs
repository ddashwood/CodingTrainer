namespace CodingTrainer.CodingTrainerModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserTheme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SelectedTheme", c => c.String(nullable: false, defaultValue: "elegant"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SelectedTheme");
        }
    }
}
