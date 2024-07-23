﻿using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Service.Interface;
using Newtonsoft.Json;
using System;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GowBoard.Controllers
{
    public class BoardController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IBoardService _boardService;
        private readonly IFileService _fileService;
        private readonly ICommentService _commentService;

        public BoardController(IMemberService memberService, IBoardService boardService, IFileService fileService, ICommentService commentService)
        {
            _memberService = memberService;
            _boardService = boardService;
            _fileService = fileService;
            _commentService = commentService;
        }

        // GET: Board/Create?category=(category)
        // 글등록 페이지
        public ActionResult Create(string category)
        {
            ViewBag.Category = category;
            if (Session["MemberId"] != null)
            {
                string memberId = Session["MemberId"].ToString();
                var member = _memberService.GetMemberById(memberId);
                var role = _memberService.GetRoleByMemberId(memberId);

                if (role.RoleName != "admin")
                {
                    if (category == "Notice")
                    {
                        TempData["ErrorMessage"] = "해당 카테고리에 관한 권한이 없습니다.";
                        return RedirectToAction("List", "Board", new { category = category });
                    }
                }

                if (member != null)
                {
                    return View(member);
                }
                ViewBag.ErrorMessage = "잘못된 접근입니다.";
                return View("Create");
                
            }
            TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
            return RedirectToAction("Login", "Member");
        }

        // POST: Board/Create
        // 글등록
        [HttpPost]
        [ValidateInput(false)] // HTML 인코딩 방지
        public async Task<ActionResult> Create(string category, ReqBoardDTO createBoardDTO)
        {

            if (Session["MemberId"] == null)
            {
                return Json(new { success = false, message = "로그인한 회원만 이용 가능합니다." });
            }
            string memberId = Session["MemberId"].ToString();
            var role = _memberService.GetRoleByMemberId(memberId);
            if (role.RoleId != 2)
            {
                if (category == "Notice")
                {
                    return Json(new { success = false, message = "해당 카테고리에 관한 권한이 없습니다." });
                }
            }
            try
            {
                await _boardService.CreateBoard(memberId, category, createBoardDTO);

                var boardContentId = createBoardDTO.BoardContentId;
                return Json(new { success = true, message = "게시물이 등록되었습니다", boardContentId });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }


        }

        // POST: Board/BoardFileCreate
        // editor 파일 등록시 파일 저장
        [HttpPost]
        public async Task<ActionResult> BoardFileCreate(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    int boardFileId = await _fileService.CreateFileAsync(file);
                    string downloadLink = Url.Action("DownloadFile", "Board", new { BoardFileId = boardFileId }, protocol: Request.Url.Scheme);

                    var response = new
                    {
                        boardFileId = boardFileId,
                        link = downloadLink
                    };

                    string jsonResponse = JsonConvert.SerializeObject(response);


                    ViewBag.BoardFileId = boardFileId;
                    return Content(jsonResponse, "application/json");

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            };

            // 파일이 제출되지 않은 경우
            return Json(new { success = false, message = "파일을 선택하세요." });
        }

        // 파일 다운로드
        [HttpGet]
        public async Task<ActionResult> DownloadFile(int boardFileId)
        {
            ResFileResult result = await _fileService.DownloadFileAsync(boardFileId);

            if (result == null || result.Resource == null)
            {
                return HttpNotFound();
            }

            var cd = new ContentDisposition
            {
                FileName = result.FileName,
                Inline = false
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(result.Resource, "application/octet-stream");
        }

        // GET: Board/List
        // 게시판 리스트 페이지
        public async Task<ActionResult> List(string category, int page = 1, int pageSize = 10, string searchType = "", string searchKeyword = "")
        {
            ViewBag.Category = category;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchType = searchType;
            ViewBag.SearchKeyword = searchKeyword;

            try
            {
                var searchBoardDTO = new ReqSearchBoardDTO
                {
                    Category = category,
                    Page = page,
                    PageSize = pageSize,
                    SearchType = searchType,
                    SearchKeyword = searchKeyword
                };
                var boardList = await _boardService.SelectAllBoardListAsync(searchBoardDTO);
                
                return View(Tuple.Create(boardList.BoardList, boardList.TotalCount, boardList.TotalPages));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "잘못된 접근입니다.";
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SearchList(ReqSearchBoardDTO searchBoardDTO)
        {
            ViewBag.Category = searchBoardDTO.Category;

            try
            {
                var boardList = await _boardService.SelectAllBoardListAsync(searchBoardDTO);

                var result = new
                {
                    TotalCount = boardList.TotalCount,
                    CurrentPage = searchBoardDTO.Page,
                    TotalPages = boardList.Item2,
                    PageSize = searchBoardDTO.PageSize,
                    BoardList = boardList.Item1
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "잘못된 접근입니다.";
                throw ex;
            }

        }

        // GET: Board/DetailView/(id)
        // 게시판 디테일 뷰 페이지
        public ActionResult DetailView(int? id)
        {
            string memberId = Session["MemberId"]?.ToString();
            var member = memberId != null ? _memberService.GetMemberById(memberId) : null;
            var role = memberId != null ? _memberService.GetRoleByMemberId(memberId) : null;

            var boardContent = _boardService.GetBoardContentById(id.Value);
            var boardComments = _commentService.GetBoardCommentListByContentId(id.Value);
            var totalCommentCount = _commentService.GetTotalCommentCount(id.Value);

            if (boardContent == null)
            {
                return HttpNotFound();
            }

            _boardService.UpdateViewCount(id.Value);

            var viewModel = new ResBoardDetailAndMemberInfo
            {
                BoardContent = boardContent,
                MemberInfoOrRole = new ResMemberInfoOrRole
                {
                    Member = member,
                    Role = role
                },
                Comments = boardComments,
                TotalCommentCount = totalCommentCount
            };

            return View(viewModel);
        }

        // GET: Board/UpdateData/(id)
        // 게시판 원글 불러오기
        public JsonResult UpdateData(int id)
        {
            string memberId = Session["MemberId"].ToString();
            var member = _memberService.GetMemberById(memberId);
            var role = _memberService.GetRoleByMemberId(memberId);
            var boardContent = _boardService.GetBoardContentById(id);

            if (boardContent == null)
            {
                return Json(new { success = false, message = "게시글을 찾을 수 없습니다." }, JsonRequestBehavior.AllowGet);
            }
;
            if (member != null)
            {
                if (role.RoleName == "admin" || boardContent.Writer.MemberId == memberId)
                {
                    return Json(new
                    {
                        success = true,
                        title = boardContent.Title,
                        content = boardContent.Content,
                        category = boardContent.Category,
                        fileIds = boardContent.BoardFileIds
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "해당 글에 관한 권한이 없습니다." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "잘못된 접근입니다." }, JsonRequestBehavior.AllowGet);
        }

        // GET: Board/Update/(id)
        // 게시글 수정 페이지
        public ActionResult Update(int id)
        {

            if (Session["MemberId"] != null)
            {
                string memberId = Session["MemberId"].ToString();
                var member = _memberService.GetMemberById(memberId);

                if (member != null)
                {
                    ViewBag.id = id;
                    return View(id);
                }
                ViewBag.ErrorMessage = "잘못된 접근입니다.";
                return View("Create");
            }
            TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
            return RedirectToAction("Login", "Member");
        }

        // POST: Board/Upadate
        // 게시글 수정
        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> Update(int id, ReqBoardDTO updateBoardDTO)
        {
            if (Session["MemberId"] == null)
            {
                return Json(new { success = false, message = "로그인한 회원만 이용 가능합니다." });
            }

            string memberId = Session["MemberId"].ToString();
            try
            {
                updateBoardDTO.BoardContentId = id;
                await _boardService.UpdateBoard(memberId, updateBoardDTO);

                return Json(new { success = true, message = "게시글이 수정되었습니다." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "서버 오류가 발생했습니다." + ex.Message });
            }
        }


        // GET: Board/Delete/(id)
        // 게시글 삭제
        public async Task<ActionResult> Delete(int id)
        {
            if (Session["MemberId"] != null)
            {
                var boardContent = _boardService.GetBoardContentById(id);

                if (boardContent == null)
                {
                    ViewBag.ErrorMessage = "게시글을 찾을 수 없습니다.";
                    return RedirectToAction("List");
                }

                var category = boardContent.Category;

                string memberId = Session["MemberId"].ToString();
                var member = _memberService.GetMemberById(memberId);
                var role = _memberService.GetRoleByMemberId(memberId);
                if (member != null)
                {
                    if (role.RoleName == "admin" || boardContent.Writer.MemberId == memberId)
                    {
                        _boardService.DeleteBoardAsync(id);
                        return RedirectToAction("List", new { category = category });
                    }
                    TempData["ErrorMessage"] = "해당 글에 관한 권한이 없습니다.";
                    return RedirectToAction("List", new { category = category });
                }
                TempData["ErrorMessage"] = "잘못된 접근입니다.";
                return RedirectToAction("List", new { category = category });
            }
            TempData["ErrorMessage"] = "로그인한 회원만 이용 가능한 페이지입니다.";
            return RedirectToAction("Login", "Member");
        }



        [HttpPost]
        public async Task<ActionResult> RemoveFile(int boardFileId)
        {
            try
            {
                bool success = await _fileService.RemoveFile(boardFileId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }





    }

}



