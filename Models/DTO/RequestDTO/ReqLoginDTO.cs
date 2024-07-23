using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GowBoard.Models.DTO.RequestDTO
{
    public class ReqLoginDTO
    {
        public string MemberId { get; set; }
        public string Password { get; set; }
    }
}