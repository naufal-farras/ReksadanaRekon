namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedTableDataReturn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataReturs", "IFUAName", c => c.String());
            AddColumn("dbo.DataReturs", "SARefrence", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataReturs", "SARefrence");
            DropColumn("dbo.DataReturs", "IFUAName");
        }
    }
}
