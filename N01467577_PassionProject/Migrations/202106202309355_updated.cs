namespace N01467577_PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartDtoes",
                c => new
                    {
                        CartId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        date = c.String(),
                    })
                .PrimaryKey(t => t.CartId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CartDtoes");
        }
    }
}
