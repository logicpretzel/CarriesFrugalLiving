namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartitems : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GCartItems", "IngredientID", c => c.Int(nullable: false));
            AddColumn("dbo.GCartItems", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.GCartItems", "UnitsID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GCartItems", "UnitsID");
            DropColumn("dbo.GCartItems", "Quantity");
            DropColumn("dbo.GCartItems", "IngredientID");
        }
    }
}
