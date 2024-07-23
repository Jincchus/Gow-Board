using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GowBoard.Models.Service.Interface
{
    public interface ICommentService
    {
        void CreateComment(string memberId, ReqBoardCommentDTO reqBoardCommentDTO);
        List<ResBoardCommentDTO> GetBoardCommentListByContentId(int boardCommentId);
        int GetTotalCommentCount(int boardCommentId);
        void UpdateCommentById(ReqUpdateCommentDTO reqBoardCommentDTO);
        string getCommentWirterById(int commentId);
        void DeleteCommentById(int commentId);
    }
}
