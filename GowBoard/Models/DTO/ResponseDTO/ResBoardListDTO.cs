using System;
using System.Collections.Generic;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResBoardListDTO
    {
        public int BoardContentId { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public ResWriterDTO Writer { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ResBoardFileDTO> Files { get; set; }
        public int CommentCount { get; set; }

        public string FormattedCreatedAt
        {
            get
            {
                return CreatedAt.ToString("yy.MM.dd tt h:mm");
            }
        }
    }
}