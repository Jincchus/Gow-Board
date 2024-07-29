using GowBoard.Models.Context;
using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Entity;
using GowBoard.Models.Service.Interface;
using GowBoard.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GowBoard.Models.Service
{
    public class CommentService : ICommentService
    {
        private readonly GowBoardContext _context;

        public CommentService(GowBoardContext context)
        {
            _context = context;
        }

        public void CreateComment(string memberId, ReqBoardCommentDTO reqBoardCommentDTO)
        {
            DateTime koreaNow = DateTimeUtility.GetKoreanNow();

            var comment = new BoardComment
            {
                BoardContentId = reqBoardCommentDTO.BoardContentId,
                WriterId = memberId,
                Content = reqBoardCommentDTO.Content,
                CreatedAt = koreaNow,
                ParentCommentId = reqBoardCommentDTO.ParentCommentId
            };

            _context.BoardCommnets.Add(comment);
            _context.SaveChanges();
        }

        public List<ResBoardCommentDTO> GetBoardCommentListByContentId(int boardContentId)
        {
            var comments = _context.BoardCommnets
                .Where(cm => cm.BoardContentId == boardContentId && cm.ParentCommentId == null)
                .Select(cm => new ResBoardCommentDTO
                {
                    BoardCommentId = cm.BoardCommentId,
                    BoardContentId = cm.BoardContentId,
                    Content = cm.Content,
                    Writer = new ResWriterDTO
                    {
                        MemberId = cm.Writer.MemberId,
                        Nickname = cm.Writer.Nickname,
                    },
                    CreatedAt = cm.CreatedAt,
                    ParentCommentId = cm.ParentCommentId
                })
                .ToList();

            foreach (var comment in comments)
            {
                comment.Replies = _context.BoardCommnets
                    .Where(r => r.ParentCommentId == comment.BoardCommentId)
                    .Select(r => new ResBoardCommentDTO
                    {
                        BoardCommentId = r.BoardCommentId,
                        BoardContentId = r.BoardContentId,
                        Content = r.Content,
                        Writer = new ResWriterDTO
                        {
                            MemberId = r.Writer.MemberId,
                            Nickname = r.Writer.Nickname,
                        },
                        CreatedAt = r.CreatedAt,
                        ParentCommentId = r.ParentCommentId
                    })
                    .ToList();
            }


            return comments;
        }
        public int GetTotalCommentCount(int boardContentId)
        {
            var parentComments = _context.BoardCommnets
                .Where(cm => cm.BoardContentId == boardContentId && cm.ParentCommentId == null)
                .ToList();

            int totalCount = parentComments.Count;
            totalCount += parentComments.Sum(pc => _context.BoardCommnets.Count(cc => cc.ParentCommentId == pc.BoardCommentId));

            return totalCount;
        }

        public void UpdateCommentById(ReqUpdateCommentDTO reqBoardCommentDTO)
        {
            DateTime koreaNow = DateTimeUtility.GetKoreanNow();
            var comment = _context.BoardCommnets.FirstOrDefault(bc => bc.BoardCommentId == reqBoardCommentDTO.BoardCommentId);
            if (comment == null)
            {
                throw new Exception("해당 댓글을 찾을 수 없습니다.");
            }

            comment.Content = reqBoardCommentDTO.Content;
            comment.ModifierId = reqBoardCommentDTO.WriterId;
            comment.ModifiedAt = koreaNow;

            _context.SaveChanges();
        }

        public string getCommentWirterById(int commentId)
        {
            return _context.BoardCommnets
                .FirstOrDefault(bc => bc.BoardCommentId == commentId)?
                .WriterId;
        }

        public void DeleteCommentById(int commentId)
        {
            var comment = _context.BoardCommnets.FirstOrDefault(bc => bc.BoardCommentId == commentId);
            if (comment == null)
            {
                throw new Exception("해당 댓글을 찾을 수 없습니다.");
            }

            DeleteChildComments(commentId);

            _context.BoardCommnets.Remove(comment);
            _context.SaveChanges();
        }

        private void DeleteChildComments(int parentCommentId)
        {
            var childComments = _context.BoardCommnets.Where(bc => bc.ParentCommentId == parentCommentId).ToList();

            if (childComments == null || !childComments.Any())
            {
                return;
            }

            foreach(var childComment in childComments)
            {
                DeleteChildComments(childComment.BoardCommentId);
                _context.BoardCommnets.Remove(childComment);
            }



        }

    }




}