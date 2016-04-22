namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class grocerycartuserid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroceryCarts", "UserID", c => c.String(maxLength: 100));
            DropColumn("dbo.GroceryCarts", "UserCD");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GroceryCarts", "UserCD", c => c.String(maxLength: 100));
            DropColumn("dbo.GroceryCarts", "UserID");
        }
    }
}
