namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipeuserid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "UserId", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "UserId");
        }
    }
}
