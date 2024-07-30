using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Entity;
using GowBoard.Models.Service.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        protected int? GetRoleId()
        {
            return Session["RoleId"] as int?;
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
                var roleId = GetRoleId();

                if (roleId != 2 && category == "Notice")
                {
                        TempData["ErrorMessage"] = "해당 카테고리에 관한 등록 권한이 없습니다.";
                        return RedirectToAction("List", "Board", new { category = category });
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
            var roleId = GetRoleId();
            if (roleId != 2)
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
        public async Task<ActionResult> BoardFileCreate(HttpPostedFileBase file, bool isEditorImage = false)
        {
            if (file != null && file.ContentLength > 0)
            {
                const int maxFileSize = 20 * 1024 * 1024; // 20MB in bytes
                if (file.ContentLength > maxFileSize)
                {
                    return Json(new { success = false, message = "20MB이상 크기의 파일을 첨부 할 수 없습니다." });
                }

                try
                {
                    int boardFileId = await _fileService.CreateFileAsync(file, isEditorImage);
                    string downloadLink = Url.Action("DownloadFile", "Board", new { BoardFileId = boardFileId }, protocol: Request.Url.Scheme);

                    var response = new
                    {
                        boardFileId = boardFileId,
                        link = downloadLink
                    };

                    return Json(response, "application/json");

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
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

            string memberId = Session["MemberId"]?.ToString();
            var roleId = GetRoleId();

            ViewBag.RoleId = roleId;
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
        public async Task<ActionResult> DetailView(int? id)
        {
            string memberId = Session["MemberId"]?.ToString();
            var member = memberId != null ? _memberService.GetMemberById(memberId) : null;
            var role = memberId != null ? _memberService.GetRoleByMemberId(memberId) : null;

            var boardContent = _boardService.GetBoardContentById(id.Value);
            var boardComments =  _commentService.GetBoardCommentListByContentId(id.Value);
            var totalCommentCount = _commentService.GetTotalCommentCount(id.Value);

            if (boardContent == null)
            {
                return HttpNotFound();
            }

            _boardService.UpdateViewCount(id.Value);

            var boardFiles = new List<ResFileResult>();
            var fileIds = boardContent.BoardFiles.Where(f => !f.IsEditorImage);
            foreach (var fileId in fileIds)
            {
                var file = await _fileService.GetFileByIdAsync(fileId.BoardFileId);
                if (file != null)
                {
                    boardFiles.Add(new ResFileResult
                    {
                        FileName = file.OriginFileName,
                        BoardFileId = file.BoardFileId,
                        IsEditorImage = file.IsEditorImage
                    });
                }
            }

            var viewModel = new ResBoardDetailAndMemberInfo
            {
                BoardContent = boardContent,
                MemberInfoOrRole = new ResMemberInfoOrRole
                {
                    Member = member,
                    Role = role
                },
                Comments = boardComments,
                TotalCommentCount = totalCommentCount,
                BoardFiles = boardFiles
            };

            return View(viewModel);
        }

        // GET: Board/UpdateData/(id)
        // 게시판 원글 불러오기
        public JsonResult UpdateData(int id)
        {
            string memberId = Session["MemberId"].ToString();
            var member = _memberService.GetMemberById(memberId);
            var roleId = GetRoleId();
            var boardContent = _boardService.GetBoardContentById(id);

            if (boardContent == null)
            {
                return Json(new { success = false, message = "게시글을 찾을 수 없습니다." }, JsonRequestBehavior.AllowGet);
            }
;
            if (member != null)
            {
                if (roleId == 2 || boardContent.Writer.MemberId == memberId)
                {
                    var attachments = boardContent.BoardFiles.Where(f => !f.IsEditorImage)
                                                  .Select(f => new { id = f.BoardFileId, name = f.FileName });

                    return Json(new
                    {
                        success = true,
                        title = boardContent.Title,
                        content = boardContent.Content,
                        category = boardContent.Category,
                        boardFiles = attachments
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
                var roleId = GetRoleId();
                if (member != null)
                {
                    if (roleId == 2 || boardContent.Writer.MemberId == memberId)
                    {
                        await _boardService.DeleteBoardAsync(id);
                        TempData["SuccessMessage"] = "게시글을 삭제했습니다.";
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
                var result = await _fileService.RemoveFileAsync(boardFileId);
                if (result)
                {
                    return Json(new { success = true, message ="파일이 성공적으로 삭제되었습니다." });
                }
                else
                {
                    return Json(new { success = false, messsage = "파일을 찾을 수 없거나 이미 삭제된 파일입니다." });
                } 
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"파일 삭제 중 오류가 발생했습니다: {ex.Message}" });
            }
        
        }

    }

}



