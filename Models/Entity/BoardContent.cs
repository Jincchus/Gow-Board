using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GowBoard.Models.Entity
{
    [Table("board_content")]
    public class BoardContent
    {
        public BoardContent()
        {
            ViewCount = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("board_content_id")]
        public int BoardContentId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("category")]
        public string Category { get; set; }

        [Required]
        [StringLength(50)]
        [Column("writer_id")]
        public string WriterId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("content")]
        [MaxLength]
        public string Content { get; set; }

        [Column("view_count")]
        public int ViewCount { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [StringLength(50)]
        [Column("modifier_id")]
        public string ModifierId { get; set; }


        [ForeignKey("WriterId")]
        public virtual Member Writer { get; set; }

        [ForeignKey("ModifierId")]
        public virtual Member Modifier { get; set; }

        public virtual ICollection<BoardFile> BoardFiles { get; set; }
        public virtual ICollection<BoardComment> BoardComments { get; set; }
    }
}