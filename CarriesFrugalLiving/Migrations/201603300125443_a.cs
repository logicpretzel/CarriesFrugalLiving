namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GroceryCarts", "Email", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GroceryCarts", "Email", c => c.String(nullable: false));
        }
    }
}
