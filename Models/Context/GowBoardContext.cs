using GowBoard.Models.Entity;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GowBoard.Models.Context
{
    public class GowBoardContext : DbContext
    {
        public GowBoardContext() : base("GowBoardContext") { }

        public DbSet<Member> Members { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<MemberRoleMap> MemberRoleMaps { get; set; }
        public DbSet<BoardContent> BoardContents { get; set; }
        public DbSet<BoardFile> BoardFiles { get; set; }
        public DbSet<BoardComment> BoardCommnets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();

            modelBuilder.Entity<MemberRoleMap>()
                .HasKey(mrm => new { mrm.MemberId, mrm.RoleId });

            modelBuilder.Entity<BoardComment>()
                .HasOptional(bc => bc.ParentComment)
                .WithMany(bc => bc.Replies)
                .HasForeignKey(bc => bc.ParentCommentId);

            // Member Entity
            modelBuilder.Entity<Member>()
                .HasMany(m => m.BoardContentsWritten)
                .WithRequired(b => b.Writer)
                .HasForeignKey(b => b.WriterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Member>()
                .HasMany(m => m.BoardContentsModified)
                .WithOptional(b => b.Modifier)
                .HasForeignKey(b => b.ModifierId)
                .WillCascadeOnDelete(false);

            // BoardContent
            modelBuilder.Entity<BoardContent>()
                .HasRequired(b => b.Writer)
                .WithMany(m => m.BoardContentsWritten)
                .HasForeignKey(b => b.WriterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BoardContent>()
                .HasOptional(b => b.Modifier)
                .WithMany(m => m.BoardContentsModified)
                .HasForeignKey(b => b.ModifierId)
                .WillCascadeOnDelete(false);



            base.OnModelCreating(modelBuilder);
        }
    }
}