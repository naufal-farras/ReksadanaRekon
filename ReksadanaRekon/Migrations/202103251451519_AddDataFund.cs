namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDataFund : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataFundReds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tanggal = c.DateTime(nullable: false),
                        Keterangan = c.String(),
                        KeteranganDua = c.String(),
                        Debit = c.Long(nullable: false),
                        Credit = c.Long(nullable: false),
                        Saldo = c.Long(),
                        MatchingId = c.Int(nullable: false),
                        RekeningId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        KeteranganUser = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matchings", t => t.MatchingId, cascadeDelete: true)
                .ForeignKey("dbo.Rekenings", t => t.RekeningId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MatchingId)
                .Index(t => t.RekeningId)
                .Index(t => t.UserId);
            
            AlterColumn("dbo.DataAplikasiReds", "PayAmount", c => c.Long(nullable: false));
            AlterColumn("dbo.DataAplikasiReds", "UnitRedeem", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DataFundReds", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataFundReds", "RekeningId", "dbo.Rekenings");
            DropForeignKey("dbo.DataFundReds", "MatchingId", "dbo.Matchings");
            DropIndex("dbo.DataFundReds", new[] { "UserId" });
            DropIndex("dbo.DataFundReds", new[] { "RekeningId" });
            DropIndex("dbo.DataFundReds", new[] { "MatchingId" });
            AlterColumn("dbo.DataAplikasiReds", "UnitRedeem", c => c.Double(nullable: false));
            AlterColumn("dbo.DataAplikasiReds", "PayAmount", c => c.Double(nullable: false));
            DropTable("dbo.DataFundReds");
        }
    }
}
