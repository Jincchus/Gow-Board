using System;
using System.Collections.Generic;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResBoardCommentDTO
    {
        public int BoardCommentId { get; set; }
        public int BoardContentId { get; set; }
        public string Content { get; set; }
        public ResWriterDTO Writer { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ParentCommentId { get; set; }
        public List<ResBoardCommentDTO> Replies { get; set; } = new List<ResBoardCommentDTO>();

        public string FormattedCreatedAt
        {
            get
            {
                return CreatedAt.ToString("yy.MM.dd tt h:mm");
            }
        }
    }
}