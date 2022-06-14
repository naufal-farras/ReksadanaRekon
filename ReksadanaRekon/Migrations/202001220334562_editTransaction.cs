namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editTransaction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "DataAplikasiId", "dbo.DataAplikasis");
            DropForeignKey("dbo.Transactions", "DataFundId", "dbo.DataFunds");
            DropIndex("dbo.Transactions", new[] { "DataFundId" });
            DropIndex("dbo.Transactions", new[] { "DataAplikasiId" });
            AddColumn("dbo.DataAplikasis", "KeteranganUser", c => c.String());
            AddColumn("dbo.DataFunds", "KeteranganDua", c => c.String());
            AddColumn("dbo.DataFunds", "KeteranganUser", c => c.String());
            AddColumn("dbo.Transactions", "KeteranganUser", c => c.String());
            AlterColumn("dbo.DataAplikasis", "TransactionDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Transactions", "DataFundId", c => c.Int());
            AlterColumn("dbo.Transactions", "DataAplikasiId", c => c.Int());
            CreateIndex("dbo.Transactions", "DataFundId");
            CreateIndex("dbo.Transactions", "DataAplikasiId");
            AddForeignKey("dbo.Transactions", "DataAplikasiId", "dbo.DataAplikasis", "Id");
            AddForeignKey("dbo.Transactions", "DataFundId", "dbo.DataFunds", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "DataFundId", "dbo.DataFunds");
            DropForeignKey("dbo.Transactions", "DataAplikasiId", "dbo.DataAplikasis");
            DropIndex("dbo.Transactions", new[] { "DataAplikasiId" });
            DropIndex("dbo.Transactions", new[] { "DataFundId" });
            AlterColumn("dbo.Transactions", "DataAplikasiId", c => c.Int(nullable: false));
            AlterColumn("dbo.Transactions", "DataFundId", c => c.Int(nullable: false));
            AlterColumn("dbo.DataAplikasis", "TransactionDate", c => c.DateTime());
            DropColumn("dbo.Transactions", "KeteranganUser");
            DropColumn("dbo.DataFunds", "KeteranganUser");
            DropColumn("dbo.DataFunds", "KeteranganDua");
            DropColumn("dbo.DataAplikasis", "KeteranganUser");
            CreateIndex("dbo.Transactions", "DataAplikasiId");
            CreateIndex("dbo.Transactions", "DataFundId");
            AddForeignKey("dbo.Transactions", "DataFundId", "dbo.DataFunds", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Transactions", "DataAplikasiId", "dbo.DataAplikasis", "Id", cascadeDelete: true);
        }
    }
}
