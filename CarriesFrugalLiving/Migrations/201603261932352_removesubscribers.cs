namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removesubscribers : DbMigration
    {
        public override void Down()
        {
            DropForeignKey("dbo.GCartItems", "GroceryCartID", "dbo.GroceryCarts");
            DropForeignKey("dbo.GroceryCarts", "SubscriberID", "dbo.Subscribers");
            DropForeignKey("dbo.MPlanItems", "MealPlanID", "dbo.MealPlans");
            DropForeignKey("dbo.MealPlans", "SubscriberID", "dbo.Subscribers");
            DropIndex("dbo.GroceryCarts", new[] { "SubscriberID" });
            DropIndex("dbo.GCartItems", new[] { "GroceryCartID" });
            DropIndex("dbo.MealPlans", new[] { "SubscriberID" });
            DropIndex("dbo.MPlanItems", new[] { "MealPlanID" });
            DropTable("dbo.Subscribers");
            DropTable("dbo.GroceryCarts");
            DropTable("dbo.GCartItems");
            DropTable("dbo.MealPlans");
            DropTable("dbo.MPlanItems");
        }
        
        public override void Up()
        {
            CreateTable(
                "dbo.MPlanItems",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MealPlanID = c.Int(nullable: false),
                        Description = c.String(),
                        MealDate = c.DateTime(nullable: false),
                        MealDay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.MealPlans",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SubscriberID = c.Int(nullable: false),
                        Name = c.String(),
                        StartDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GCartItems",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GroceryCartID = c.Int(nullable: false),
                        Description = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GroceryCarts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserCD = c.String(nullable: false, maxLength: 100),
                        SubscriberID = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Name = c.String(maxLength: 80),
                    })
                .PrimaryKey(t => t.ID);
            
            //CreateTable(
            //    "dbo.Subscribers",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            LastName = c.String(maxLength: 50),
            //            FirstName = c.String(maxLength: 50),
            //            City = c.String(maxLength: 50),
            //            State = c.String(),
            //            Email = c.String(maxLength: 150),
            //            AccountCd = c.Guid(nullable: false),
            //            MemberSince = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.MPlanItems", "MealPlanID");
         //   CreateIndex("dbo.MealPlans", "SubscriberID");
            CreateIndex("dbo.GCartItems", "GroceryCartID");
          //  CreateIndex("dbo.GroceryCarts", "SubscriberID");
          //  AddForeignKey("dbo.MealPlans", "SubscriberID", "dbo.Subscribers", "ID", cascadeDelete: true);
            AddForeignKey("dbo.MPlanItems", "MealPlanID", "dbo.MealPlans", "ID", cascadeDelete: true);
          //  AddForeignKey("dbo.GroceryCarts", "SubscriberID", "dbo.Subscribers", "ID", cascadeDelete: true);
            AddForeignKey("dbo.GCartItems", "GroceryCartID", "dbo.GroceryCarts", "ID", cascadeDelete: true);
        }
    }
}
