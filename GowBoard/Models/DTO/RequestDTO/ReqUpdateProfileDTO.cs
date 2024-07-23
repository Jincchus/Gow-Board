using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GowBoard.Models.DTO.RequestDTO
{
    public class ReqUpdateProfileDTO
    {
        public string OriginPassword {get; set;}
        public string Password { get; set;}
        public string Email { get; set;}
        public string Nickname { get; set;}
        public string Phone { get; set;}
    }
}