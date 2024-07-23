using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.DTO.ResponseDTO.Home;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GowBoard.Models.Service.Interface
{
    public interface IBoardService
    {
        Task CreateBoard(string memberId, string category, ReqBoardDTO createBoardDTO);
        Task<(List<ResBoardListDTO> BoardList, int TotalCount, int TotalPages)> SelectAllBoardListAsync(ReqSearchBoardDTO reqBoardSearchListDTO);
        ResBoardDetailDTO GetBoardContentById(int boardContentId);
        void UpdateViewCount(int boardContentId);
        Task UpdateBoard(string memberId, ReqBoardDTO updateBoardDTO);
        Task DeleteBoardAsync(int boardContentId);
        Task<int> GetTotalCountAsync(string caregory);
        Task<(List<DateTime>, List<int>)> GetDailyBoardCountAsync(string caregory, int days);

        Task<List<ResPostRankDTO>> GetTopViewNoticeLastMonth();
        Task<List<ResPostRankDTO>> GetNewPostByCategory(string category);
        Task<List<ResPostRankDTO>> GetTopFiveFreeBoards();
    }
}
