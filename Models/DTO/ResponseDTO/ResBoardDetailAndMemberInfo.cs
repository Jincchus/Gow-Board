using System.Collections.Generic;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResBoardDetailAndMemberInfo
    {
        public ResBoardDetailDTO BoardContent { get; set; }
        public ResMemberInfoOrRole MemberInfoOrRole { get; set; }
        public List<ResBoardCommentDTO> Comments { get; set; }
        public int TotalCommentCount { get; set; }

    }

}