namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gcart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroceryCarts", "UserCD", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.GroceryCarts", "Name", c => c.String(maxLength: 80));
            AlterColumn("dbo.GCartItems", "Description", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GCartItems", "Description", c => c.String());
            AlterColumn("dbo.GroceryCarts", "Name", c => c.String());
            DropColumn("dbo.GroceryCarts", "UserCD");
        }
    }
}
