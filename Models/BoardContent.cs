using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GowBoard.Models
{
    public class BoardContent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BoardContentId { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string WriterId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }


        public int viewCount { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime createdAt { get; set; }


        public DateTime modifiedAt { get; set; }


        public string modifierId { get; set; }
    }
}