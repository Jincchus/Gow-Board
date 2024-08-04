using GowBoard.Models.Context;
using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.Service;
using GowBoard.Models.Service.Interface;
using System;
using System.Web.Mvc;

namespace GowBoard.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;

        public MemberController() { }

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }


        // GET: Member
        public ActionResult Index()
        {
            return View();
        }

        // GET: Member/Register
        // 회원가입 페이지
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Post: Member/Register
        // 회원가입
        [HttpPost]

        public ActionResult Register(ReqRegisterDTO registerDto)
        {
            var registered = _memberService.RegisterMember(registerDto);

            return Json(new { success = registered.Success, message = registered.Message });
        }

        // POST: MEMBER/DuplicatedCheckId
        // 아이디 중복검사
        [HttpPost]
        public ActionResult DuplicatedCheckId(string memberId)
        {
            var isDuplicate = _memberService.DuplicatedCheckId(memberId);
            return Json(new { success = isDuplicate.Success, message = isDuplicate.Message });
        }

        // POST: MEMBER/DuplicatedCheckNickname
        // 닉네임 중복검사
        [HttpPost]
        public ActionResult DuplicatedCheckNickname(string nickname)
        {
            var isDuplicate = _memberService.DuplicatedCheckNickname(nickname);
            return Json(new { success = isDuplicate.Success, message = isDuplicate.Message });
        }


        // POST: Member/SendAuthenticationEmail
        // 이메일 인증번호 전송
        public ActionResult SendAuthenticationEmail(string email)
        {


            var isDuplicate = _memberService.DuplicatedCheckEmail(email);
            if (!isDuplicate.Success)
            {
                return Json(new { success = isDuplicate.Success, message = isDuplicate.Message });
            }

            var result = _memberService.SendAuthenticationEmail(email);
            bool emailSent = result.Item1;
            string authNumber = result.Item2;


            return Json(new { success = emailSent, authNumber = authNumber });
        }

        // GET: Member/LogIn
        // 로그인

        [HttpGet]
        public ActionResult LogIn()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View();
        }


        // POST: Member/LogIn
        // 로그인
        [HttpPost]
        public ActionResult LogIn(ReqLoginDTO loginDto)
        {
            var loginResult = _memberService.Login(loginDto);
            if (!loginResult.IsSuccess)
            {
                if (loginResult.IsDeleted)
                {
                    ViewBag.ErrorMessage = "해당 아이디는 회원탈퇴가 진행중인 아이디입니다.";
                }
                else
                {
                    ViewBag.ErrorMessage = "입력하신 아이디 혹은 비밀번호가 올바르지 않습니다";
                }

                    return View("Login", loginDto);
            }

            Session["MemberId"] = loginResult.Member.MemberId;
            Session["Nickname"] = loginResult.Member.Nickname;

            var role = _memberService.GetRoleByMemberId(loginResult.Member.MemberId);
            Session["RoleId"] = role.RoleId;

            return RedirectToAction("Index", "Home");

        }

        // GET: Member/LogOut
        // 로그아웃
        public ActionResult LogOut()
        {
            Session.Remove("MemberId");
            Session.Remove("RoleId");
            Session.Remove("Nickname");
            return RedirectToAction("LogIn", "Member");

        }

        // GET: Member/FindId
        // 아이디 찾기
        public ActionResult FindId()
        {
            return View();
        }

        // GET: Member/NewPassword
        // 비밀번호 재설정
        public ActionResult NewPassword()
        {
            return View();
        }
    }
}