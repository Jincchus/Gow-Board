using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GowBoard.Models.Entity
{
    [Table("board_file")]
    public class BoardFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("board_file_id")]
        public int BoardFileId { get; set; }

        [Column("board_content_id")]
        public int? BoardContentId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("origin_file_name")]
        public string OriginFileName { get; set; }


        [Required]
        [StringLength(255)]
        [Column("save_file_name")]
        public string SaveFileName { get; set; }

        [Required]
        [StringLength(10)]
        [Column("extension")]
        public string Extension { get; set; }

        [Required]
        [Column("file_size")]
        public int FileSize { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("is_editor_image")]
        public bool IsEditorImage { get; set; }

        [ForeignKey("BoardContentId")]
        public virtual BoardContent BoardContent { get; set; }
    }
}