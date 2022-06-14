namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRed_Swi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataAplikasiReds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CIF = c.String(),
                        CIF_APERD = c.String(),
                        AccNum = c.String(),
                        HolderName = c.String(),
                        RedempNo = c.String(),
                        TransDate = c.DateTime(nullable: false),
                        PayAmount = c.Double(nullable: false),
                        UnitRedeem = c.Double(nullable: false),
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
                "dbo.FundTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Nama = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FundTypes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataAplikasiReds", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataAplikasiReds", "SAId", "dbo.SAs");
            DropForeignKey("dbo.DataAplikasiReds", "MIId", "dbo.MIs");
            DropForeignKey("dbo.DataAplikasiReds", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.DataAplikasiReds", "FundId", "dbo.Funds");
            DropIndex("dbo.FundTypes", new[] { "UserId" });
            DropIndex("dbo.DataAplikasiReds", new[] { "UserId" });
            DropIndex("dbo.DataAplikasiReds", new[] { "MIId" });
            DropIndex("dbo.DataAplikasiReds", new[] { "FundId" });
            DropIndex("dbo.DataAplikasiReds", new[] { "SAId" });
            DropIndex("dbo.DataAplikasiReds", new[] { "MatchingId" });
            DropTable("dbo.FundTypes");
            DropTable("dbo.DataAplikasiReds");
        }
    }
}
