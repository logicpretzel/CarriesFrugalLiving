namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reviewcng : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reviews", "UserID", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reviews", "UserID", c => c.Int(nullable: false));
        }
    }
}
