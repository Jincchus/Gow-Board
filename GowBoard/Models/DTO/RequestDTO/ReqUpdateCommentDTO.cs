using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GowBoard.Models.DTO.RequestDTO
{
    public class ReqUpdateCommentDTO
    {
        public int? BoardCommentId { get; set; }
        public string Content { get; set; }
        public string WriterId { get; set; }
    }
}