namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abusereport2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbuseReports", "ReviewID", c => c.Int(defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbuseReports", "ReviewID");
        }
    }
}
