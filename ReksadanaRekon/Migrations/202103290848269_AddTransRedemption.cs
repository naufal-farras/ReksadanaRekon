namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransRedemption : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransRedemps",
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
                "dbo.TrRedAplikasis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransRedempId = c.Int(nullable: false),
                        DataAplikasiRedId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataAplikasiReds", t => t.DataAplikasiRedId, cascadeDelete: false)
                .ForeignKey("dbo.TransRedemps", t => t.TransRedempId, cascadeDelete: true)
                .Index(t => t.TransRedempId)
                .Index(t => t.DataAplikasiRedId);
            
            CreateTable(
                "dbo.TrRedFunds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransRedempId = c.Int(nullable: false),
                        DataFundRedId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataFundReds", t => t.DataFundRedId, cascadeDelete: false)
                .ForeignKey("dbo.TransRedemps", t => t.TransRedempId, cascadeDelete: true)
                .Index(t => t.TransRedempId)
                .Index(t => t.DataFundRedId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrRedFunds", "TransRedempId", "dbo.TransRedemps");
            DropForeignKey("dbo.TrRedFunds", "DataFundRedId", "dbo.DataFundReds");
            DropForeignKey("dbo.TrRedAplikasis", "TransRedempId", "dbo.TransRedemps");
            DropForeignKey("dbo.TrRedAplikasis", "DataAplikasiRedId", "dbo.DataAplikasiReds");
            DropForeignKey("dbo.TransRedemps", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.TransRedemps", "InputerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TransRedemps", "ApproverId", "dbo.AspNetUsers");
            DropIndex("dbo.TrRedFunds", new[] { "DataFundRedId" });
            DropIndex("dbo.TrRedFunds", new[] { "TransRedempId" });
            DropIndex("dbo.TrRedAplikasis", new[] { "DataAplikasiRedId" });
            DropIndex("dbo.TrRedAplikasis", new[] { "TransRedempId" });
            DropIndex("dbo.TransRedemps", new[] { "ApproverId" });
            DropIndex("dbo.TransRedemps", new[] { "InputerId" });
            DropIndex("dbo.TransRedemps", new[] { "MatchingId" });
            DropTable("dbo.TrRedFunds");
            DropTable("dbo.TrRedAplikasis");
            DropTable("dbo.TransRedemps");
        }
    }
}
