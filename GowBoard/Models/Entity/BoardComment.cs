using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GowBoard.Models.Entity
{
    [Table("board_comment")]
    public class BoardComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("board_comment_id")]
        public int BoardCommentId { get; set; }

        [Required]
        [Column("board_content_id")]
        public int BoardContentId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("writer_id")]
        public string WriterId { get; set; }

        [Required]
        [Column("content", TypeName = "nvarchar(MAX)")]
        public string Content { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("parent_comment_id")]
        public int? ParentCommentId { get; set; }

        [Column("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [StringLength(50)]
        [Column("modifier_id")]
        public string ModifierId { get; set; }


        [ForeignKey("BoardContentId")]
        public virtual BoardContent BoardContent { get; set; }

        [ForeignKey("WriterId")]
        public virtual Member Writer { get; set; }


        [ForeignKey("ModifierId")]
        public virtual Member Modifier { get; set; }

        [ForeignKey("ParentCommentId")]
        public virtual BoardComment ParentComment { get; set; }


        public virtual ICollection<BoardComment> Replies { get; set; }
    }

}