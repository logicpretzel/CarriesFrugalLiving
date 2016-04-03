namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abusereport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AbuseReports",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RecipeID = c.Int(nullable: false, defaultValue: 0),
                        UserCD = c.String(),
                        CreateDate = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                        AbuseType = c.Int(nullable: false, defaultValue: 0),
                        Comment = c.String(maxLength: 1024),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Recipes", t => t.RecipeID, cascadeDelete: true)
                .Index(t => t.RecipeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AbuseReports", "RecipeID", "dbo.Recipes");
            DropIndex("dbo.AbuseReports", new[] { "RecipeID" });
            DropTable("dbo.AbuseReports");
        }
    }
}
