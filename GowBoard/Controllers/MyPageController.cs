using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Service.Interface;
using System.Web.Mvc;

namespace GowBoard.Controllers
{
    public class MyPageController : Controller
    {
        private readonly IMemberService _memberService;

        public MyPageController(IMemberService memberService)
        {
            _memberService = memberService;
        }


        // GET: MyPage/MyProfile
        // 프로필 페이지
        public ActionResult MyProfile()
        {
            if (Session["MemberId"] != null)
            {
                // 해당 세션 정보(memberId)로 회원 조회
                string memberId = Session["MemberId"].ToString();
                var member = _memberService.GetMemberById(memberId);
                var role = _memberService.GetRoleByMemberId(memberId);

                if (member != null && role != null)
                {
                    var memberAndRole = new ResMemberInfoOrRole
                    {
                        Member = member,
                        Role = role
                    };
                    return View(memberAndRole);
                }
                else
                {
                    ViewBag.ErrorMessage = "잘못된 접근입니다.";
                    return View("MyProfile");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
                return RedirectToAction("Login", "Member");
            }
        }

        // Get: MyPage/UpdateProfile
        // 프로필 수정 페이지
        public ActionResult UpdateProfile()
        {
            if (Session["MemberId"] != null)
            {
                // 해당 세션 정보(memberId)로 회원 조회
                string memberId = Session["MemberId"].ToString();
                var member = _memberService.GetMemberById(memberId);
                var role = _memberService.GetRoleByMemberId(memberId);

                if (member != null && role != null)
                {
                    var memberAndRole = new ResMemberInfoOrRole
                    {
                        Member = member,
                        Role = role
                    };
                    return View(memberAndRole);
                }
                else
                {
                    ViewBag.ErrorMessage = "잘못된 접근입니다.";
                    return View("UpdateProfile");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
                return RedirectToAction("Login", "Member");
            }
        }

        // POST : MyPage/UpdateProfile
        // 프로필 수정
        [HttpPost]
        public ActionResult UpdateProfile(ReqUpdateProfileDTO reqUpdateProfileDto)
        {
            if (Session["MemberId"] == null)
            {
                TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
                return RedirectToAction("Login", "Member");
            }

            // session으로 originMemberInfo 조회
            string memberId = Session["MemberId"].ToString();
            var originMemberInfo = _memberService.GetMemberById(memberId);

            if (originMemberInfo == null)
            {
                ViewBag.ErrorMessage = "잘못된 접근입니다.";
                return View("UpdateProfile");
            }

            var updateResult = _memberService.UpdateMemberProfile(originMemberInfo, reqUpdateProfileDto);
            return Json(new { success = updateResult.Success, message = updateResult.Message });
        }

        // Get: MyPage/Withdrawal
        // 회원 탈퇴 페이지
        [HttpGet]
        public ActionResult Withdrawal()
        {
            if (Session["MemberId"] != null)
            {
                // 해당 세션 정보(memberId)로 회원 조회
                string memberId = Session["MemberId"].ToString();
                var member = _memberService.GetMemberById(memberId);
                var role = _memberService.GetRoleByMemberId(memberId);

                if (TempData["ErrorMessage"] != null)
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"];
                    return View(member);
                }

                if (member != null && role != null)
                {
                    var memberAndRole = new ResMemberInfoOrRole
                    {
                        Member = member,
                        Role = role
                    };
                    return View(memberAndRole);
                }
                else
                {
                    ViewBag.ErrorMessage = "잘못된 접근입니다.";
                    return View("MyProfile");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
                return RedirectToAction("Login", "Member");
            }
        }

        // POST: MyPage/Withdrawal
        // 회원 탈퇴
        [HttpPost]
        public ActionResult Withdrawal(string password)
        {
            string memberId = Session["MemberId"].ToString();
            bool isValidCredentials = _memberService.VerifyCredentials(memberId, password);

            if (!isValidCredentials)
            {

                TempData["ErrorMessage"] = "비밀번호가 일치하지않습니다";
                return RedirectToAction("Withdrawal");
            }

            _memberService.DeleteMember(memberId);
            Session.Remove("MemberId");

            TempData["SuccessMessage"] = "회원탈퇴 신청이 완료되었습니다. 30일 이후에는 해당 아이디를 복구하실 수 없습니다. ";
            return RedirectToAction("Index", "Home");
        }


        // Get: MyPage/MyCommentList
        // 내가 쓴 댓글 리스트 페이지
        public ActionResult MyCommentList()
        {
            return View();
        }

        // Get: MyPage/MyPostList
        // 내가 쓴 게시글 리스트 페이지
        public ActionResult MyPostList()
        {
            return View();
        }
    }
}