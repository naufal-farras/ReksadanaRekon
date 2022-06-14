namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyDataFund : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DataFunds", "NoRek");
            DropColumn("dbo.DataFunds", "NamaRek");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DataFunds", "NamaRek", c => c.String());
            AddColumn("dbo.DataFunds", "NoRek", c => c.String());
        }
    }
}
