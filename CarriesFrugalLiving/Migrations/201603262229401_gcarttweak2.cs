namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gcarttweak2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GroceryCarts", "Email", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GroceryCarts", "Email", c => c.String());
        }
    }
}
