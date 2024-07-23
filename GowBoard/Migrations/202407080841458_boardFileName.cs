namespace GowBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class boardFileName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.board_file", "origin_file_name", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.board_file", "save_file_name", c => c.String(nullable: false, maxLength: 255));
            DropColumn("dbo.board_file", "file_name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.board_file", "file_name", c => c.String(nullable: false, maxLength: 255));
            DropColumn("dbo.board_file", "save_file_name");
            DropColumn("dbo.board_file", "origin_file_name");
        }
    }
}
