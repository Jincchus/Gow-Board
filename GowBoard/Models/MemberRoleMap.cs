using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GowBoard.Models
{
    public class MemberRoleMap
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string MemberId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int RoleId { get; set; }

        public Member Member { get; set; }
        public Role Role { get; set; }

    }
}