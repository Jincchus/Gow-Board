using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GowBoard.Models.Entity
{
    [Table("member_role_map")]
    public class MemberRoleMap
    {
        [Key]
        [Column("member_id", Order = 1)]
        [StringLength(50)]
        public string MemberId { get; set; }

        [Key]
        [Column("role_id", Order = 2)]
        public int RoleId { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

    }
}