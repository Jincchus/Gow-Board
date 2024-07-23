using System;

namespace GowBoard.Models.DTO.ResponseDTO.Home
{
    public class ResPostRankDTO
    {
        public int BoardContentId { get; set; }
        public string Category {  get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public ResWriterDTO Writer { get; set; }
        public ResBoardFileDTO Files { get; set; }

        public string FormattedCreatedAt
        {
            get
            {
                return CreatedAt.ToString("yy.MM.dd tt h:mm");
            }
        }

    }
}