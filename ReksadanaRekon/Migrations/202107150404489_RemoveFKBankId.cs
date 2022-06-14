namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFKBankId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DataReturs", "BankId", "dbo.Banks");
            DropIndex("dbo.DataReturs", new[] { "BankId" });
            DropColumn("dbo.DataReturs", "BankId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DataReturs", "BankId", c => c.Int(nullable: false));
            CreateIndex("dbo.DataReturs", "BankId");
            AddForeignKey("dbo.DataReturs", "BankId", "dbo.Banks", "Id", cascadeDelete: true);
        }
    }
}
