using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GowBoard.Models
{
    public class Member
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string MemberId { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Nickname { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool DeleteYn { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}