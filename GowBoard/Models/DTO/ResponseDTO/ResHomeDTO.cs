using GowBoard.Models.DTO.ResponseDTO.Home;
using System.Collections.Generic;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResHomeDTO
    {
        public List<ResPostRankDTO> TopViewNotice { get; set; }
        public List<ResPostRankDTO> NewNotices { get; set; }
        public List<ResPostRankDTO> NewFreeBoards { get; set; }
        public List<ResPostRankDTO> TopFiveBoards { get; set; }
    }
}