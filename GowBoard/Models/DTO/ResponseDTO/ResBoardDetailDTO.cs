using GowBoard.Models.DTO.RequestDTO;
using System;
using System.Collections.Generic;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResBoardDetailDTO
    {
        public int BoardContentId { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public ResWriterDTO Writer { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ResFileResult> BoardFiles { get; set; }

        public string FormattedCreatedAt
        {
            get
            {
                return CreatedAt.ToString("yy.MM.dd tt h:mm");
            }
        }
    }
}