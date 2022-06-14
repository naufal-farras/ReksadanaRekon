namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrDataRetur : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrDataReturs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransaksiId = c.Int(nullable: false),
                        DataReturId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DataReturs", t => t.DataReturId, cascadeDelete: false)
                .ForeignKey("dbo.Transaksis", t => t.TransaksiId, cascadeDelete: true)
                .Index(t => t.TransaksiId)
                .Index(t => t.DataReturId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrDataReturs", "TransaksiId", "dbo.Transaksis");
            DropForeignKey("dbo.TrDataReturs", "DataReturId", "dbo.DataReturs");
            DropIndex("dbo.TrDataReturs", new[] { "DataReturId" });
            DropIndex("dbo.TrDataReturs", new[] { "TransaksiId" });
            DropTable("dbo.TrDataReturs");
        }
    }
}
