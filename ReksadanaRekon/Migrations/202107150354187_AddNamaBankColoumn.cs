namespace ReksadanaRekon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNamaBankColoumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataReturs", "NamaBank", c => c.String());
            Sql("UPDATE DR SET DR.NamaBank = B.Nama FROM DataReturs AS DR INNER JOIN Banks AS B ON DR.BankId = B.Id");
        }

        public override void Down()
        {
            DropColumn("dbo.DataReturs", "NamaBank");
        }
    }
}
