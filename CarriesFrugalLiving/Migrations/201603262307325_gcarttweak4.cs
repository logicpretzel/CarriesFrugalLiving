namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gcarttweak4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GroceryCarts", "UserCD", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GroceryCarts", "UserCD", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
