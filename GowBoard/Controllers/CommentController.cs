//using GowBoard.Hubs;
using GowBoard.Hubs;
using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.Service.Interface;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GowBoard.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IMemberService _memberService;
        private readonly IBoardService _boardService;
        private readonly INotificationService _notificationService;

        public CommentController(ICommentService commentService, IMemberService memberService, IBoardService boardService, INotificationService notificationService)
        {
            _commentService = commentService;
            _memberService = memberService;
            _boardService = boardService;
            _notificationService = notificationService;
        }

        // Post: Comment/CreateComment
        // 댓글 등록
        public async Task<JsonResult> CreateComment(ReqBoardCommentDTO reqBoardCommentDTO)
        {


            if (Session["MemberId"] == null)
            {
                return Json(new { isAuthenticated = false, message = "로그인한 회원만 이용 가능합니다." });
            }

            string memberId = Session["MemberId"].ToString();

            try
            {
                _commentService.CreateComment(memberId, reqBoardCommentDTO);
                var boardContentInfo = _boardService.GetBoardContentById(reqBoardCommentDTO.BoardContentId);
                _notificationService.NotifyUser(boardContentInfo.Writer.MemberId, "새로운 댓글이 달렸습니다.");

                // TODO : 대댓글시 댓글(parentId) 탐색 후 알림 전송
                if (reqBoardCommentDTO.ParentCommentId != null)
                {
                    // var parentCommentWriter = await _commentService.GetCommentWriterByIdAsync(reqBoardCommentDTO.ParentCommentId.Value);
                    // await _notificationService.SendNotificationAsync(parentCommentWriter, "새로운 답댓글이 달렸습니다.");
                }


                return Json(new { success = true, message = "댓글이 등록되었습니다." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "댓글 등록에 실패하였습니다." });
            }
        }

        [HttpPost]
        public JsonResult Update(ReqUpdateCommentDTO reqUpdateCommentDTO)
        {
            if (Session["MemberId"] == null)
            {
                return Json(new { isAuthenticated = false, message = "로그인한 회원만 이용 가능합니다." });
            }

            string memberId = Session["MemberId"]?.ToString();
            var member = _memberService.GetMemberById(memberId);
            if (member == null)
            {
                return Json(new { success = false, message = "해당 회원을 찾을 수 없습니다." });
            }

            var role = _memberService.GetRoleByMemberId(memberId);
            if (role == null || role.RoleId != 2 && memberId != reqUpdateCommentDTO.WriterId)
            {
                return Json(new { success = false, message = "해당 글에 관한 수정 권한이 없습니다." });
            }

            try
            {
                _commentService.UpdateCommentById(reqUpdateCommentDTO);
                return Json(new { success = true, message = "댓글이 수정되었습니다." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

            }
        }

        [HttpPost]
        public JsonResult Delete(int commentId)
        {
            string memberId = Session["MemberId"]?.ToString();
            if (string.IsNullOrEmpty(memberId))
            {
                return Json(new { success = false, message = " 로그인한 회원만 이용 가능한 페이지입니다." });
            }

            var commentWriter = _commentService.getCommentWirterById(commentId);
            if (commentWriter == null)
            {
                return Json(new { success = false, message = "해당 댓글을 찾을 수 없습니다." });
            }

            var member = _memberService.GetMemberById(memberId);
            var role = _memberService.GetRoleByMemberId(memberId);
            if (member == null || role == null ||
                role.RoleId != 2 && memberId != commentWriter)
            {
                return Json(new { success = false, message = "해당 글에 관한 삭제 권한이 없습니다." });
            }

            _commentService.DeleteCommentById(commentId);
            return Json(new { success = true, message = "댓글을 삭제했습니다." });
        }





    }
}