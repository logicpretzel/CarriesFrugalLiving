namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class picklist2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PickLists",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    KeyId = c.Int(nullable: false),
                    CatId = c.Int(nullable: false),
                    Description = c.String(maxLength:1024),
                    Void = c.Boolean(nullable: false, defaultValue: false),
                })
                .PrimaryKey(t => t.ID);
        }
        
        public override void Down()
        {
        }
    }
}
