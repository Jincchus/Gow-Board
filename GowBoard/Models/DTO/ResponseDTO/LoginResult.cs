using GowBoard.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public bool IsDeleted { get; set; }
        public Member Member {  get; set; }
    }
}