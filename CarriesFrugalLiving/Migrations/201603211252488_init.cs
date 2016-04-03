namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.mFractions",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    text = c.String(),
                    num = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.id);

            AddColumn("dbo.Ingredients", "qtyWhole", c => c.Int(nullable: false));
            AddColumn("dbo.Ingredients", "qtyFraction", c => c.Int(nullable: false));
            AddColumn("dbo.Reviews", "Offensive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Reviews", "ReportedByUserCD", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "Address", c => c.String());
            AddColumn("dbo.AspNetUsers", "Zip", c => c.String(maxLength: 10));
            AlterColumn("dbo.Ingredients", "Description", c => c.String(maxLength: 4000));
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "City", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "State", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "State", c => c.String());
            AlterColumn("dbo.AspNetUsers", "City", c => c.String());
            AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Ingredients", "Description", c => c.String());
            DropColumn("dbo.AspNetUsers", "Zip");
            DropColumn("dbo.AspNetUsers", "Address");
            DropColumn("dbo.Reviews", "ReportedByUserCD");
            DropColumn("dbo.Reviews", "Offensive");
            DropColumn("dbo.Ingredients", "qtyFraction");
            DropColumn("dbo.Ingredients", "qtyWhole");
            DropTable("dbo.mFractions");
        }
    }
}
