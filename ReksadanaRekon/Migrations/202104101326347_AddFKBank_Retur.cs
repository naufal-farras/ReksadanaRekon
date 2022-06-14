namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFKBank_Retur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataReturs", "BankId", c => c.Int(nullable: false));
            CreateIndex("dbo.DataReturs", "BankId");
            AddForeignKey("dbo.DataReturs", "BankId", "dbo.Banks", "Id", cascadeDelete: true);
            DropColumn("dbo.DataReturs", "Bank");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DataReturs", "Bank", c => c.String());
            DropForeignKey("dbo.DataReturs", "BankId", "dbo.Banks");
            DropIndex("dbo.DataReturs", new[] { "BankId" });
            DropColumn("dbo.DataReturs", "BankId");
        }
    }
}
