namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newtransaction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transaksis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MatchingId = c.Int(nullable: false),
                        InputerId = c.String(maxLength: 128),
                        KeteranganInputer = c.String(),
                        ApproverId = c.String(maxLength: 128),
                        KeteranganApprover = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApproverId)
                .ForeignKey("dbo.AspNetUsers", t => t.InputerId)
                .ForeignKey("dbo.Matchings", t => t.MatchingId, cascadeDelete: true)
                .Index(t => t.MatchingId)
                .Index(t => t.InputerId)
                .Index(t => t.ApproverId);
            
            CreateTable(
                "dbo.TrDataAplikasis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransaksiId = c.Int(nullable: false),
                        DataAplikasiId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataAplikasis", t => t.DataAplikasiId, cascadeDelete: false)
                .ForeignKey("dbo.Transaksis", t => t.TransaksiId, cascadeDelete: true)
                .Index(t => t.TransaksiId)
                .Index(t => t.DataAplikasiId);
            
            CreateTable(
                "dbo.TrDataFunds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransaksiId = c.Int(nullable: false),
                        DataFundId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataFunds", t => t.DataFundId, cascadeDelete: false)
                .ForeignKey("dbo.Transaksis", t => t.TransaksiId, cascadeDelete: true)
                .Index(t => t.TransaksiId)
                .Index(t => t.DataFundId);
            
            AddColumn("dbo.DataAplikasis", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.Funds", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.MIs", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.SAs", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.DataFunds", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.Rekenings", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.Transactions", "IsDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrDataFunds", "TransaksiId", "dbo.Transaksis");
            DropForeignKey("dbo.TrDataFunds", "DataFundId", "dbo.DataFunds");
            DropForeignKey("dbo.TrDataAplikasis", "TransaksiId", "dbo.Transaksis");
            DropForeignKey("dbo.TrDataAplikasis", "DataAplikasiId", "dbo.DataAplikasis");
            DropForeignKey("dbo.Transaksis", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.Transaksis", "InputerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transaksis", "ApproverId", "dbo.AspNetUsers");
            DropIndex("dbo.TrDataFunds", new[] { "DataFundId" });
            DropIndex("dbo.TrDataFunds", new[] { "TransaksiId" });
            DropIndex("dbo.TrDataAplikasis", new[] { "DataAplikasiId" });
            DropIndex("dbo.TrDataAplikasis", new[] { "TransaksiId" });
            DropIndex("dbo.Transaksis", new[] { "ApproverId" });
            DropIndex("dbo.Transaksis", new[] { "InputerId" });
            DropIndex("dbo.Transaksis", new[] { "MatchingId" });
            DropColumn("dbo.Transactions", "IsDelete");
            DropColumn("dbo.Rekenings", "IsDelete");
            DropColumn("dbo.DataFunds", "IsDelete");
            DropColumn("dbo.SAs", "IsDelete");
            DropColumn("dbo.MIs", "IsDelete");
            DropColumn("dbo.Funds", "IsDelete");
            DropColumn("dbo.DataAplikasis", "IsDelete");
            DropTable("dbo.TrDataFunds");
            DropTable("dbo.TrDataAplikasis");
            DropTable("dbo.Transaksis");
        }
    }
}
