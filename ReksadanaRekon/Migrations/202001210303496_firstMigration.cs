namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataAplikasis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionDate = c.DateTime(),
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
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Funds", t => t.FundId, cascadeDelete: true)
                .ForeignKey("dbo.Matchings", t => t.MatchingId, cascadeDelete: true)
                .ForeignKey("dbo.MIs", t => t.MIId, cascadeDelete: true)
                .ForeignKey("dbo.SAs", t => t.SAId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MatchingId)
                .Index(t => t.SAId)
                .Index(t => t.FundId)
                .Index(t => t.MIId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Funds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Nama = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nama = c.String(nullable: false),
                        NPP = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Matchings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nama = c.String(),
                        Keterangan = c.String(),
                        Warna = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MIs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Nama = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SAs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Nama = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DataFunds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NoRek = c.String(),
                        NamaRek = c.String(),
                        CCY = c.String(),
                        Tanggal = c.DateTime(nullable: false),
                        Keterangan = c.String(),
                        Jumlah = c.Long(nullable: false),
                        Saldo = c.Long(nullable: false),
                        MatchingId = c.Int(nullable: false),
                        RekeningId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matchings", t => t.MatchingId, cascadeDelete: true)
                .ForeignKey("dbo.Rekenings", t => t.RekeningId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MatchingId)
                .Index(t => t.RekeningId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Rekenings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NoRek = c.String(),
                        NamaRek = c.String(),
                        SAId = c.Int(),
                        FundId = c.Int(),
                        MIId = c.Int(),
                        UserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Funds", t => t.FundId)
                .ForeignKey("dbo.MIs", t => t.MIId)
                .ForeignKey("dbo.SAs", t => t.SAId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.SAId)
                .Index(t => t.FundId)
                .Index(t => t.MIId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Kelompoks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NamaKelompok = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataFundId = c.Int(nullable: false),
                        DataAplikasiId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataAplikasis", t => t.DataAplikasiId, cascadeDelete: true)
                .ForeignKey("dbo.DataFunds", t => t.DataFundId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.DataFundId)
                .Index(t => t.DataAplikasiId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Transactions", "DataFundId", "dbo.DataFunds");
            DropForeignKey("dbo.Transactions", "DataAplikasiId", "dbo.DataAplikasis");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DataFunds", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataFunds", "RekeningId", "dbo.Rekenings");
            DropForeignKey("dbo.Rekenings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Rekenings", "SAId", "dbo.SAs");
            DropForeignKey("dbo.Rekenings", "MIId", "dbo.MIs");
            DropForeignKey("dbo.Rekenings", "FundId", "dbo.Funds");
            DropForeignKey("dbo.DataFunds", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.DataAplikasis", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataAplikasis", "SAId", "dbo.SAs");
            DropForeignKey("dbo.SAs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataAplikasis", "MIId", "dbo.MIs");
            DropForeignKey("dbo.MIs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataAplikasis", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.DataAplikasis", "FundId", "dbo.Funds");
            DropForeignKey("dbo.Funds", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Transactions", new[] { "UserId" });
            DropIndex("dbo.Transactions", new[] { "DataAplikasiId" });
            DropIndex("dbo.Transactions", new[] { "DataFundId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Rekenings", new[] { "UserId" });
            DropIndex("dbo.Rekenings", new[] { "MIId" });
            DropIndex("dbo.Rekenings", new[] { "FundId" });
            DropIndex("dbo.Rekenings", new[] { "SAId" });
            DropIndex("dbo.DataFunds", new[] { "UserId" });
            DropIndex("dbo.DataFunds", new[] { "RekeningId" });
            DropIndex("dbo.DataFunds", new[] { "MatchingId" });
            DropIndex("dbo.SAs", new[] { "UserId" });
            DropIndex("dbo.MIs", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Funds", new[] { "UserId" });
            DropIndex("dbo.DataAplikasis", new[] { "UserId" });
            DropIndex("dbo.DataAplikasis", new[] { "MIId" });
            DropIndex("dbo.DataAplikasis", new[] { "FundId" });
            DropIndex("dbo.DataAplikasis", new[] { "SAId" });
            DropIndex("dbo.DataAplikasis", new[] { "MatchingId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Kelompoks");
            DropTable("dbo.Rekenings");
            DropTable("dbo.DataFunds");
            DropTable("dbo.SAs");
            DropTable("dbo.MIs");
            DropTable("dbo.Matchings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Funds");
            DropTable("dbo.DataAplikasis");
        }
    }
}
