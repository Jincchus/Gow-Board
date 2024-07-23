namespace GowBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowNullBoardContentId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.board_file", "board_content_id", "dbo.board_content");
            DropIndex("dbo.board_file", new[] { "board_content_id" });
            AlterColumn("dbo.board_file", "board_content_id", c => c.Int());
            CreateIndex("dbo.board_file", "board_content_id");
            AddForeignKey("dbo.board_file", "board_content_id", "dbo.board_content", "board_content_id");
            DropColumn("dbo.board_file", "blob_url");
            DropColumn("dbo.board_file", "file_mime_type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.board_file", "file_mime_type", c => c.String(maxLength: 50));
            AddColumn("dbo.board_file", "blob_url", c => c.String(nullable: false));
            DropForeignKey("dbo.board_file", "board_content_id", "dbo.board_content");
            DropIndex("dbo.board_file", new[] { "board_content_id" });
            AlterColumn("dbo.board_file", "board_content_id", c => c.Int(nullable: false));
            CreateIndex("dbo.board_file", "board_content_id");
            AddForeignKey("dbo.board_file", "board_content_id", "dbo.board_content", "board_content_id", cascadeDelete: true);
        }
    }
}
