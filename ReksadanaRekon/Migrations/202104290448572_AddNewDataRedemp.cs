namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewDataRedemp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataRedemps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransDate = c.DateTime(nullable: false),
                        ReferenceNo = c.String(),
                        Nominal = c.Long(nullable: false),
                        Nasabah = c.String(),
                        MatchingId = c.Int(nullable: false),
                        SAId = c.Int(),
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
                .ForeignKey("dbo.SAs", t => t.SAId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MatchingId)
                .Index(t => t.SAId)
                .Index(t => t.FundId)
                .Index(t => t.MIId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DataRedemps", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DataRedemps", "SAId", "dbo.SAs");
            DropForeignKey("dbo.DataRedemps", "MIId", "dbo.MIs");
            DropForeignKey("dbo.DataRedemps", "MatchingId", "dbo.Matchings");
            DropForeignKey("dbo.DataRedemps", "FundId", "dbo.Funds");
            DropIndex("dbo.DataRedemps", new[] { "UserId" });
            DropIndex("dbo.DataRedemps", new[] { "MIId" });
            DropIndex("dbo.DataRedemps", new[] { "FundId" });
            DropIndex("dbo.DataRedemps", new[] { "SAId" });
            DropIndex("dbo.DataRedemps", new[] { "MatchingId" });
            DropTable("dbo.DataRedemps");
        }
    }
}
