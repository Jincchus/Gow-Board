using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Service.Interface;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GowBoard.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IBoardService _boardService;

        public AdminController(IMemberService memberService, IBoardService boardService)
        {
            _memberService = memberService;
            _boardService = boardService;
        }


        // GET: Admin/Dashboard
        // 관리자 대시보드페이지
        public async Task<ActionResult> Dashboard()
        {
            if (Session["MemberId"] == null)
            {
                TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
                return RedirectToAction("Login", "Member");
            }
            string memberId = Session["MemberId"].ToString();
            var member = _memberService.GetMemberById(memberId);
            var role = _memberService.GetRoleByMemberId(memberId);

            if (role.RoleId != 2)
            {
                TempData["ErrorMessage"] = "해당 페이지에 관한 권한이 없습니다.";
                return RedirectToAction("Index", "Home");
            }

            var memberCount = await _memberService.GetTotalMemberCountAsync();
            var boardCount = await _boardService.GetTotalCountAsync("Board");
            var noticeCount = await _boardService.GetTotalCountAsync("Notice");
            int days = 30;
            var (dates, dailyMemberCounts) = await _memberService.GetDailyMemberCountAsync(days);
            var (_, dailtBoardCounts) = await _boardService.GetDailyBoardCountAsync("Board", days);
            var (_, dailyNoticeCounts) = await _boardService.GetDailyBoardCountAsync("Notice", days);
            // 일 별 멤버 통계
            // 일 별 카테고리별 글 통계

            var dashboardInfo = new ResDashboardDTO
            {

                MemberCount = memberCount,
                BoardCount = boardCount,
                NoticeCount = noticeCount,
                Dates = dates,
                DailyMemberCount = dailyMemberCounts,
                DailyBoardCount = dailtBoardCounts,
                DailyNoticeCount = dailyNoticeCounts
            };


            ViewBag.Member = member;
            ViewBag.Role = role;
            ViewBag.CurrentPage = "Dashboard";

            return View(dashboardInfo);
        }

        public async Task<ActionResult> MemberList()
        {
            if (Session["MemberId"] == null)
            {
                TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
                return RedirectToAction("Login", "Member");
            }

            string memberId = Session["MemberId"].ToString();
            var member = _memberService.GetMemberById(memberId);
            var role = _memberService.GetRoleByMemberId(memberId);

            if (role.RoleId != 2)
            {
                TempData["ErrorMessage"] = "해당 페이지에 관한 권한이 없습니다.";
                return RedirectToAction("Index", "Home");
            }

            var memberList = await _memberService.GetMemberList();

            ViewBag.Member = member;
            ViewBag.Role = role;
            ViewBag.CurrentPage = "MemberList";

            return View(memberList);
        }
    }
}