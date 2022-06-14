namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAplikasiRed_Redemp : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrRedAplikasis", "DataAplikasiRedId", "dbo.DataAplikasiReds");
            DropIndex("dbo.TrRedAplikasis", new[] { "DataAplikasiRedId" });
            AddColumn("dbo.TrRedAplikasis", "DataRedempId", c => c.Int(nullable: false));
            CreateIndex("dbo.TrRedAplikasis", "DataRedempId");
            AddForeignKey("dbo.TrRedAplikasis", "DataRedempId", "dbo.DataRedemps", "Id", cascadeDelete: false);
            DropColumn("dbo.TrRedAplikasis", "DataAplikasiRedId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrRedAplikasis", "DataAplikasiRedId", c => c.Int(nullable: false));
            DropForeignKey("dbo.TrRedAplikasis", "DataRedempId", "dbo.DataRedemps");
            DropIndex("dbo.TrRedAplikasis", new[] { "DataRedempId" });
            DropColumn("dbo.TrRedAplikasis", "DataRedempId");
            CreateIndex("dbo.TrRedAplikasis", "DataAplikasiRedId");
            AddForeignKey("dbo.TrRedAplikasis", "DataAplikasiRedId", "dbo.DataAplikasiReds", "Id", cascadeDelete: true);
        }
    }
}
