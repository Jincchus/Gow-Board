namespace GowBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.board_file", "is_editor_image", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.board_file", "is_editor_image");
        }
    }
}
