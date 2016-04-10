namespace CarriesFrugalLiving.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class features2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Features", "url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Features", "url");
        }
    }
}
