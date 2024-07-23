using GowBoard.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResMemberInfoOrRole
    {
        public Member Member { get; set; }
        public Role Role { get; set; }
    }
}