namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRetur : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nama = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        DeleteDate = c.DateTime(),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DataReturs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransDate = c.DateTime(nullable: false),
                        NoRekening = c.String(),
                        NamaNasabah = c.String(),
                        Bank = c.String(),
                        Nominal = c.Long(nullable: false),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DataReturs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataReturs", "SAId", "dbo.SAs");
            DropForeignKey("dbo.DataReturs", "MIId", "dbo.MIs");
            DropForeignKey("dbo.DataReturs", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.DataReturs", "FundId", "dbo.Funds");
            DropIndex("dbo.DataReturs", new[] { "UserId" });
            DropIndex("dbo.DataReturs", new[] { "MIId" });
            DropIndex("dbo.DataReturs", new[] { "FundId" });
            DropIndex("dbo.DataReturs", new[] { "SAId" });
            DropIndex("dbo.DataReturs", new[] { "MatchingId" });
            DropTable("dbo.DataReturs");
            DropTable("dbo.Banks");
        }
    }
}
