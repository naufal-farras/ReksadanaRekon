namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKeteranganRetur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataRedemps", "KeteranganRetur", c => c.String());
            AddColumn("dbo.DataReturs", "KeteranganRetur", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataReturs", "KeteranganRetur");
            DropColumn("dbo.DataRedemps", "KeteranganRetur");
        }
    }
}
