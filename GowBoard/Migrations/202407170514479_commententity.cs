namespace GowBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commententity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.board_comment", "modified_at", c => c.DateTime());
            AddColumn("dbo.board_comment", "modifier_id", c => c.String(maxLength: 50));
            CreateIndex("dbo.board_comment", "modifier_id");
            AddForeignKey("dbo.board_comment", "modifier_id", "dbo.member", "member_id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.board_comment", "modifier_id", "dbo.member");
            DropIndex("dbo.board_comment", new[] { "modifier_id" });
            DropColumn("dbo.board_comment", "modifier_id");
            DropColumn("dbo.board_comment", "modified_at");
        }
    }
}
