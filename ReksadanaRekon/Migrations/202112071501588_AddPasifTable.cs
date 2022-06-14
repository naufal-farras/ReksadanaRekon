namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPasifTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataAplikasiPasifs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        TransactionDate = c.DateTime(nullable: false),
                        TransactionType = c.String(),
                        ReferenceNo = c.String(),
                        Status = c.String(),
                        IMFeeAmendment = c.String(),
                        IMPaymentDate = c.String(),
                        SACode = c.String(),
                        SAName = c.String(),
                        InvestorFundUnitNo = c.String(),
                        InvestorFundUnitName = c.String(),
                        SID = c.String(),
                        FundCode = c.String(),
                        FundName = c.String(),
                        IMCode = c.String(),
                        IMName = c.String(),
                        CBCode = c.String(),
                        CBName = c.String(),
                        FundCCY = c.String(),
                        AmountNominal = c.Long(nullable: false),
                        AmountUnit = c.String(),
                        AmountAll = c.String(),
                        FeeNominal = c.String(),
                        FeeUnit = c.String(),
                        FeePercent = c.String(),
                        TransferPath = c.String(),
                        REDMSequentialCode = c.String(),
                        REDMBICCode = c.String(),
                        REDMBIMemberCode = c.String(),
                        REDMBankName = c.String(),
                        REDMNo = c.String(),
                        REDMName = c.String(),
                        PaymentDate = c.DateTime(),
                        TransferType = c.String(),
                        InputDate = c.DateTime(),
                        UploadReference = c.String(),
                        SAReference = c.String(),
                        IMStatus = c.String(),
                        CBStatus = c.String(),
                        CBCompletionStatus = c.String(),
                        MatchingId = c.Int(nullable: false),
                        SAId = c.Int(nullable: false),
                        FundId = c.Int(nullable: false),
                        MIId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        KeteranganUser = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Funds", t => t.FundId, cascadeDelete: false)
                .ForeignKey("dbo.Matchings", t => t.MatchingId, cascadeDelete: false)
                .ForeignKey("dbo.MIs", t => t.MIId, cascadeDelete: false)
                .ForeignKey("dbo.SAs", t => t.SAId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MatchingId)
                .Index(t => t.SAId)
                .Index(t => t.FundId)
                .Index(t => t.MIId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DataFundPasifs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        CCY = c.String(),
                        Tanggal = c.DateTime(nullable: false),
                        Keterangan = c.String(),
                        Jumlah = c.Long(nullable: false),
                        Saldo = c.Long(nullable: false),
                        MatchingId = c.Int(nullable: false),
                        RekeningId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        KeteranganDua = c.String(),
                        KeteranganUser = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matchings", t => t.MatchingId, cascadeDelete: false)
                .ForeignKey("dbo.Rekenings", t => t.RekeningId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MatchingId)
                .Index(t => t.RekeningId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TransaksiPasifs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        MatchingId = c.Int(nullable: false),
                        InputerId = c.String(maxLength: 128),
                        KeteranganInputer = c.String(),
                        ApproverId = c.String(maxLength: 128),
                        KeteranganApprover = c.String(),
                        Retur = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApproverId)
                .ForeignKey("dbo.AspNetUsers", t => t.InputerId)
                .ForeignKey("dbo.Matchings", t => t.MatchingId, cascadeDelete: false)
                .Index(t => t.MatchingId)
                .Index(t => t.InputerId)
                .Index(t => t.ApproverId);
            
            CreateTable(
                "dbo.TrDataAplikasiPasifs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        TransaksiId = c.Int(nullable: false),
                        DataAplikasiId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataAplikasiPasifs", t => t.DataAplikasiId, cascadeDelete: false)
                .ForeignKey("dbo.TransaksiPasifs", t => t.TransaksiId, cascadeDelete: false)
                .Index(t => t.TransaksiId)
                .Index(t => t.DataAplikasiId);
            
            CreateTable(
                "dbo.TrDataFundPasifs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        TransaksiId = c.Int(nullable: false),
                        DataFundId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataFundPasifs", t => t.DataFundId, cascadeDelete: false)
                .ForeignKey("dbo.TransaksiPasifs", t => t.TransaksiId, cascadeDelete: false)
                .Index(t => t.TransaksiId)
                .Index(t => t.DataFundId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrDataFundPasifs", "TransaksiId", "dbo.TransaksiPasifs");
            DropForeignKey("dbo.TrDataFundPasifs", "DataFundId", "dbo.DataFundPasifs");
            DropForeignKey("dbo.TrDataAplikasiPasifs", "TransaksiId", "dbo.TransaksiPasifs");
            DropForeignKey("dbo.TrDataAplikasiPasifs", "DataAplikasiId", "dbo.DataAplikasiPasifs");
            DropForeignKey("dbo.TransaksiPasifs", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.TransaksiPasifs", "InputerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TransaksiPasifs", "ApproverId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataFundPasifs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataFundPasifs", "RekeningId", "dbo.Rekenings");
            DropForeignKey("dbo.DataFundPasifs", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.DataAplikasiPasifs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataAplikasiPasifs", "SAId", "dbo.SAs");
            DropForeignKey("dbo.DataAplikasiPasifs", "MIId", "dbo.MIs");
            DropForeignKey("dbo.DataAplikasiPasifs", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.DataAplikasiPasifs", "FundId", "dbo.Funds");
            DropIndex("dbo.TrDataFundPasifs", new[] { "DataFundId" });
            DropIndex("dbo.TrDataFundPasifs", new[] { "TransaksiId" });
            DropIndex("dbo.TrDataAplikasiPasifs", new[] { "DataAplikasiId" });
            DropIndex("dbo.TrDataAplikasiPasifs", new[] { "TransaksiId" });
            DropIndex("dbo.TransaksiPasifs", new[] { "ApproverId" });
            DropIndex("dbo.TransaksiPasifs", new[] { "InputerId" });
            DropIndex("dbo.TransaksiPasifs", new[] { "MatchingId" });
            DropIndex("dbo.DataFundPasifs", new[] { "UserId" });
            DropIndex("dbo.DataFundPasifs", new[] { "RekeningId" });
            DropIndex("dbo.DataFundPasifs", new[] { "MatchingId" });
            DropIndex("dbo.DataAplikasiPasifs", new[] { "UserId" });
            DropIndex("dbo.DataAplikasiPasifs", new[] { "MIId" });
            DropIndex("dbo.DataAplikasiPasifs", new[] { "FundId" });
            DropIndex("dbo.DataAplikasiPasifs", new[] { "SAId" });
            DropIndex("dbo.DataAplikasiPasifs", new[] { "MatchingId" });
            DropTable("dbo.TrDataFundPasifs");
            DropTable("dbo.TrDataAplikasiPasifs");
            DropTable("dbo.TransaksiPasifs");
            DropTable("dbo.DataFundPasifs");
            DropTable("dbo.DataAplikasiPasifs");
        }
    }
}
