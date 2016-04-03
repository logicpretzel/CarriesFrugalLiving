namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gcarttweak : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroceryCarts", "email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroceryCarts", "email");
        }
    }
}
