namespace GowBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.board_file", "is_editor_image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.board_file", "is_editor_image", c => c.Boolean(nullable: false));
        }
    }
}
