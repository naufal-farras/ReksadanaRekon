namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_NamaReksadana_DataRetur : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataReturs", "NamaReksadana", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataReturs", "NamaReksadana");
        }
    }
}
