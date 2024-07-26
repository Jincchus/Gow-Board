namespace GowBoard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteRoleCreatedAt : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.role", "created_at");
        }
        
        public override void Down()
        {
            AddColumn("dbo.role", "created_at", c => c.DateTime(nullable: false));
        }
    }
}
