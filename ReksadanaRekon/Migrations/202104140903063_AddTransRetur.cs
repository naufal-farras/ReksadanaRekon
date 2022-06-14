namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransRetur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transaksis", "Retur", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transaksis", "Retur");
        }
    }
}
