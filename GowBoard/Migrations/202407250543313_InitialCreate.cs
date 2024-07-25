namespace GowBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.board_comment",
                c => new
                    {
                        board_comment_id = c.Int(nullable: false, identity: true),
                        board_content_id = c.Int(nullable: false),
                        writer_id = c.String(nullable: false, maxLength: 50),
                        content = c.String(nullable: false),
                        created_at = c.DateTime(nullable: false),
                        parent_comment_id = c.Int(),
                        modified_at = c.DateTime(),
                        modifier_id = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.board_comment_id)
                .ForeignKey("dbo.board_content", t => t.board_content_id, cascadeDelete: true)
                .ForeignKey("dbo.member", t => t.modifier_id)
                .ForeignKey("dbo.board_comment", t => t.parent_comment_id)
                .ForeignKey("dbo.member", t => t.writer_id, cascadeDelete: true)
                .Index(t => t.board_content_id)
                .Index(t => t.writer_id)
                .Index(t => t.parent_comment_id)
                .Index(t => t.modifier_id);
            
            CreateTable(
                "dbo.board_content",
                c => new
                    {
                        board_content_id = c.Int(nullable: false, identity: true),
                        category = c.String(nullable: false, maxLength: 20),
                        writer_id = c.String(nullable: false, maxLength: 50),
                        title = c.String(nullable: false, maxLength: 255),
                        content = c.String(nullable: false),
                        view_count = c.Int(nullable: false),
                        created_at = c.DateTime(nullable: false),
                        modified_at = c.DateTime(),
                        modifier_id = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.board_content_id)
                .ForeignKey("dbo.member", t => t.modifier_id)
                .ForeignKey("dbo.member", t => t.writer_id)
                .Index(t => t.writer_id)
                .Index(t => t.modifier_id);
            
            CreateTable(
                "dbo.board_file",
                c => new
                    {
                        board_file_id = c.Int(nullable: false, identity: true),
                        board_content_id = c.Int(),
                        origin_file_name = c.String(nullable: false, maxLength: 255),
                        save_file_name = c.String(nullable: false, maxLength: 255),
                        extension = c.String(nullable: false, maxLength: 10),
                        file_size = c.Int(nullable: false),
                        created_at = c.DateTime(nullable: false),
                        is_editor_image = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.board_file_id)
                .ForeignKey("dbo.board_content", t => t.board_content_id)
                .Index(t => t.board_content_id);
            
            CreateTable(
                "dbo.member",
                c => new
                    {
                        member_id = c.String(nullable: false, maxLength: 50),
                        password = c.String(nullable: false, maxLength: 255),
                        name = c.String(nullable: false, maxLength: 50),
                        email = c.String(nullable: false, maxLength: 255),
                        nickname = c.String(nullable: false, maxLength: 50),
                        phone = c.String(maxLength: 20),
                        created_at = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        delete_yn = c.Boolean(nullable: false),
                        deleted_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.member_id)
                .Index(t => t.email, unique: true)
                .Index(t => t.nickname, unique: true);
            
            CreateTable(
                "dbo.member_role_map",
                c => new
                    {
                        member_id = c.String(nullable: false, maxLength: 50),
                        role_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.member_id, t.role_id })
                .ForeignKey("dbo.member", t => t.member_id, cascadeDelete: true)
                .ForeignKey("dbo.role", t => t.role_id, cascadeDelete: true)
                .Index(t => t.member_id)
                .Index(t => t.role_id);
            
            CreateTable(
                "dbo.role",
                c => new
                    {
                        role_id = c.Int(nullable: false, identity: true),
                        role_name = c.String(nullable: false, maxLength: 20),
                        created_at = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.role_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.board_comment", "writer_id", "dbo.member");
            DropForeignKey("dbo.board_comment", "parent_comment_id", "dbo.board_comment");
            DropForeignKey("dbo.board_comment", "modifier_id", "dbo.member");
            DropForeignKey("dbo.board_content", "writer_id", "dbo.member");
            DropForeignKey("dbo.board_content", "modifier_id", "dbo.member");
            DropForeignKey("dbo.member_role_map", "role_id", "dbo.role");
            DropForeignKey("dbo.member_role_map", "member_id", "dbo.member");
            DropForeignKey("dbo.board_file", "board_content_id", "dbo.board_content");
            DropForeignKey("dbo.board_comment", "board_content_id", "dbo.board_content");
            DropIndex("dbo.member_role_map", new[] { "role_id" });
            DropIndex("dbo.member_role_map", new[] { "member_id" });
            DropIndex("dbo.member", new[] { "nickname" });
            DropIndex("dbo.member", new[] { "email" });
            DropIndex("dbo.board_file", new[] { "board_content_id" });
            DropIndex("dbo.board_content", new[] { "modifier_id" });
            DropIndex("dbo.board_content", new[] { "writer_id" });
            DropIndex("dbo.board_comment", new[] { "modifier_id" });
            DropIndex("dbo.board_comment", new[] { "parent_comment_id" });
            DropIndex("dbo.board_comment", new[] { "writer_id" });
            DropIndex("dbo.board_comment", new[] { "board_content_id" });
            DropTable("dbo.role");
            DropTable("dbo.member_role_map");
            DropTable("dbo.member");
            DropTable("dbo.board_file");
            DropTable("dbo.board_content");
            DropTable("dbo.board_comment");
        }
    }
}
