namespace CodingTrainer.CodingTrainerModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFirstNameLastName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false, defaultValue: "John"));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false, defaultValue: "Doe"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
