namespace N01467577_PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProductPictureField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ProductHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "PicExtension");
            DropColumn("dbo.Products", "ProductHasPic");
        }
    }
}
