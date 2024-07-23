using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Service.Interface;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GowBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBoardService _boardService;

        public HomeController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        public async Task<ActionResult> Index()
        {
            // 한달 이내 ViewCount가 가장 높은 공지사항 1개
            var topViewNoticeLastMonth = await _boardService.GetTopViewNoticeLastMonth();

            // 카테고리 별 최신글 3개
            var newPostByBoard = await _boardService.GetNewPostByCategory("Board");
            var newPostByNotice = await _boardService.GetNewPostByCategory("Notice");

            // 자유게시판 viewcount 가장 높은순 5개
            var topFiveFreeBoards = await _boardService.GetTopFiveFreeBoards();

            var rank = new ResHomeDTO
            {
                TopViewNotice = topViewNoticeLastMonth,
                NewNotices = newPostByNotice,
                NewFreeBoards = newPostByBoard,
                TopFiveBoards = topFiveFreeBoards
            };


            return View(rank);
        }

    }
}