namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ingredient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ingredients", "sortorder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ingredients", "sortorder");
        }
    }
}
