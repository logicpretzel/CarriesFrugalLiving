namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class features1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Features", "StartDate", c => c.DateTime());
            AlterColumn("dbo.Features", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Features", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Features", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
