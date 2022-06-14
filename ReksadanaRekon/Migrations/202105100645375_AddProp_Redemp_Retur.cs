namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProp_Redemp_Retur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataRedemps", "ByInput", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.DataReturs", "RekeningNasabah", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataReturs", "RekeningNasabah");
            DropColumn("dbo.DataRedemps", "ByInput");
        }
    }
}
