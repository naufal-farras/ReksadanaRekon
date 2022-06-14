namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyReturCol : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataReturs", "PaymentDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DataReturs", "IFUA", c => c.String());
            AddColumn("dbo.DataReturs", "NoJurnal", c => c.String());
            DropColumn("dbo.DataReturs", "NamaReksadana");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DataReturs", "NamaReksadana", c => c.String());
            DropColumn("dbo.DataReturs", "NoJurnal");
            DropColumn("dbo.DataReturs", "IFUA");
            DropColumn("dbo.DataReturs", "PaymentDate");
        }
    }
}
