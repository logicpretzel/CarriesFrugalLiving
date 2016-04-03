namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gcartsmealplans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MealPlans", "UserCD", c => c.String());
            DropColumn("dbo.MealPlans", "SubscriberID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MealPlans", "SubscriberID", c => c.Int(nullable: false));
            DropColumn("dbo.MealPlans", "UserCD");
        }
    }
}
