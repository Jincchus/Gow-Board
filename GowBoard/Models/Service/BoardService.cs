using GowBoard.Models.Context;
using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.DTO.ResponseDTO.Home;
using GowBoard.Models.Entity;
using GowBoard.Models.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace GowBoard.Models.Service
{
    public class BoardService : IBoardService
    {
        private readonly GowBoardContext _context;
        private readonly IFileService _fileService;
        private readonly IMemberService _memberService;
        private readonly ICommentService _commentService;

        public BoardService(GowBoardContext context, IFileService uploadService)
        {
            _context = context;
            _fileService = uploadService;
        }

        public async Task CreateBoard(string memberId, string category, ReqBoardDTO createBoardDTO)
        {
            BoardContent board = null;
            try
            {
                board = new BoardContent
                {
                    Category = category,
                    WriterId = memberId,
                    Title = createBoardDTO.Title,
                    Content = createBoardDTO.Content,
                    CreatedAt = DateTime.Now
                };

                _context.BoardContents.Add(board);
                await _context.SaveChangesAsync();

                createBoardDTO.BoardContentId = board.BoardContentId;

                var boardFileId = createBoardDTO.BoardFileId;
                if (boardFileId != null)
                {
                    await UpdateBoardFiles(board.BoardContentId, boardFileId);
                }
            }
            catch
            {
                if (board != null && board.BoardContentId != 0)
                {
                    _context.BoardContents.Remove(board);
                    await _context.SaveChangesAsync();
                }
                throw;
            }
        }

        private async Task UpdateBoardFiles(int boardContentId, List<int> boardFileIds)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (int boardFileId in boardFileIds)
                    {
                        await _fileService.UpdateFileId(new BoardFile
                        {
                            BoardFileId = boardFileId,
                            BoardContentId = boardContentId
                        });
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public ResBoardDetailDTO GetBoardContentById(int boardContentId)
        {
            var boardContentData = _context.BoardContents
                .Where(b => b.BoardContentId == boardContentId)
                .Select(b => new
                {
                    BoardContentId = b.BoardContentId,
                    Title = b.Title,
                    Content = b.Content,
                    Category = b.Category,
                    Writer = new ResWriterDTO
                    {
                        MemberId = b.Writer.MemberId,
                        Nickname = b.Writer.Nickname
                    },
                    CreatedAt = b.CreatedAt,
                    BoardFiles = b.BoardFiles.Select(f => new ResFileResult
                    {
                        BoardFileId = f.BoardFileId,
                        FileName = f.OriginFileName
                    }).ToList()
                })
                .FirstOrDefault();

            if (boardContentData == null)
            {
                return null;
            }


            var boardContent = new ResBoardDetailDTO
            {
                BoardContentId = boardContentData.BoardContentId,
                Title = boardContentData.Title,
                Content = boardContentData.Content,
                Writer = boardContentData.Writer,
                Category = boardContentData.Category,
                CreatedAt = boardContentData.CreatedAt,
                BoardFiles = boardContentData.BoardFiles
            };

            return boardContent;

        }

        public async Task<(List<ResBoardListDTO> BoardList, int TotalCount, int TotalPages)> SelectAllBoardListAsync(ReqSearchBoardDTO searchBoardDTO)
        {
            var query = _context.BoardContents
                .Include(bc => bc.Writer)
                .Include(bc => bc.BoardFiles)
                .Include(bc => bc.BoardComments)
                .Where(bc => bc.Category == searchBoardDTO.Category)
                .AsNoTracking();

            // 검색 조건 적용
            if (!string.IsNullOrEmpty(searchBoardDTO.SearchKeyword))
            {
                switch (searchBoardDTO.SearchType)
                {
                    case "1": // 제목
                        query = query.Where(bc => bc.Title.Contains(searchBoardDTO.SearchKeyword));
                        break;
                    case "2": // 내용
                        query = query.Where(bc => bc.Content.Contains(searchBoardDTO.SearchKeyword));
                        break;
                    case "3": // 작성자
                        query = query.Where(bc => bc.Writer.Nickname.Contains(searchBoardDTO.SearchKeyword));
                        break;
                    case "4": // 댓글
                        query = query.Where(bc => bc.BoardComments.Any(c => c.Content.Contains(searchBoardDTO.SearchKeyword)));
                        break;
                    default: // 전체
                        query = query.Where(bc =>
                            bc.Title.Contains(searchBoardDTO.SearchKeyword) ||
                            bc.Content.Contains(searchBoardDTO.SearchKeyword) ||
                            bc.Writer.Nickname.Contains(searchBoardDTO.SearchKeyword) ||
                            bc.BoardComments.Any(c => c.Content.Contains(searchBoardDTO.SearchKeyword)));
                        
                        break;
                }
            }

            query = query.OrderByDescending(bc => bc.CreatedAt);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)searchBoardDTO.PageSize);

            var boardContents = await query
                .Skip((searchBoardDTO.Page - 1) * searchBoardDTO.PageSize)
                .Take(searchBoardDTO.PageSize)
                .Select(bc => new ResBoardListDTO
                {
                    BoardContentId = bc.BoardContentId,
                    Title = bc.Title,
                    ViewCount = bc.ViewCount,
                    Writer = new ResWriterDTO
                    {
                        MemberId = bc.Writer.MemberId,
                        Nickname = bc.Writer.Nickname,
                    },
                    CreatedAt = bc.CreatedAt,
                    Files = bc.BoardFiles
                        .Where(bf => new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" }.Contains(bf.Extension.ToLower()))
                        .OrderByDescending(bf => bf.BoardFileId)
                        .Take(1)
                        .Select(bf => new ResBoardFileDTO
                        {
                            ThumbUrl = bf.SaveFileName + "Thumb" + bf.Extension,
                            FileName = bf.SaveFileName,
                            Extension = bf.Extension,
                            FileSize = bf.FileSize,
                        }).ToList(),
                    CommentCount = bc.BoardComments.Count(),
                })
                .ToListAsync();

            return (boardContents,totalCount ,totalPages);
        }

        public async Task<int> GetTotalCountAsync(string caregory)
        {
            int total = await _context.BoardContents.CountAsync(x => x.Category == caregory);

            return total;
        }

        public void UpdateViewCount(int boardContentId)
        {
            var baordContent = _context.BoardContents.FirstOrDefault(b => b.BoardContentId == boardContentId);
            if (baordContent != null)
            {
                baordContent.ViewCount++;
                _context.SaveChanges();
            }

        }

        public async Task UpdateBoard(string memberId, ReqBoardDTO updateBoardDTO)
        {
            BoardContent board = null;

            try
            {
                board = await _context.BoardContents.FindAsync(updateBoardDTO.BoardContentId);
                if (board == null)
                {
                    throw new Exception("해당 게시글을 찾을 수 없습니다.");
                }

                if (board.WriterId != memberId)
                {
                    var userRole = _context.MemberRoleMaps
                                           .Where(m => m.MemberId == memberId)
                                           .Select(m => m.RoleId)
                                           .FirstOrDefault();

                    if (userRole != 2) // Assuming 2 is the role ID for admin
                    {
                        throw new UnauthorizedAccessException("수정 권한이 없습니다.");
                    }
                }

                board.ModifierId = memberId;
                board.Title = updateBoardDTO.Title;
                board.Content = updateBoardDTO.Content;
                board.ModifiedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                try
                {
                    var boardFileId = updateBoardDTO.BoardFileId;
                    if (boardFileId != null)
                    {
                        await UpdateBoardFiles(board.BoardContentId, boardFileId);
                    }
                }
                catch
                {
                    if (board != null && board.BoardContentId != 0)
                    {
                        _context.BoardContents.Remove(board);
                        _context.SaveChanges();
                    }
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteBoardAsync(int boardContentId)
        {
            var boardContent = await _context.BoardContents.FindAsync(boardContentId);
            if (boardContent == null)
            {
                throw new Exception("해당 게시글을 찾을 수 없습니다.");
            }

            var boardFiles = _context.BoardFiles.Where(f => f.BoardContentId == boardContentId).ToList();
            _context.BoardFiles.RemoveRange(boardFiles);

            var boardComments = _context.BoardCommnets.Where(c => c.BoardContentId == boardContentId).ToList();
            _context.BoardCommnets.RemoveRange(boardComments);

            _context.BoardContents.Remove(boardContent);

            await _context.SaveChangesAsync();
        }

        public async Task<(List<DateTime>, List<int>)> GetDailyBoardCountAsync(string category, int days)
        {
            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddDays(-days + 1);

            var dailyCounts = await _context.BoardContents
                .Where(bc => bc.Category == category && DbFunctions.TruncateTime(bc.CreatedAt) >= startDate && DbFunctions.TruncateTime(bc.CreatedAt) <= endDate)
                .GroupBy(m => DbFunctions.TruncateTime(m.CreatedAt))
                .Select(g => new { Date = g.Key.Value, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            var dates = Enumerable.Range(0, days)
                .Select(i => startDate.AddDays(i))
                .ToList();

            var counts = dates.Select(date => dailyCounts.FirstOrDefault(d => d.Date == date)?.Count ?? 0).ToList();

            return (dates, counts);
        }


        public async Task<List<ResPostRankDTO>> GetTopViewNoticeLastMonth()
        {
            var oneMonth = DateTime.Now.AddMonths(-1);

            return await _context.BoardContents
                .Where(bc => bc.Category == "Notice" && bc.CreatedAt >= oneMonth)
                .OrderByDescending(bc => bc.ViewCount)
                .Take(2)
                .Select(bc => new ResPostRankDTO
                {
                    BoardContentId = bc.BoardContentId,
                    Title = bc.Title,
                    CreatedAt = bc.CreatedAt,
                    Category = bc.Category,
                    Writer = new ResWriterDTO
                    {
                        MemberId = bc.WriterId,
                        Nickname = bc.Writer.Nickname
                    },
                    Files = bc.BoardFiles
                    .OrderByDescending(bf => bf.BoardFileId)
                    .Take(1)
                    .Select(f => new ResBoardFileDTO
                    {
                        ThumbUrl = f.SaveFileName + f.Extension,
                        FileName = f.SaveFileName,
                        Extension = f.Extension,
                        FileSize = f.FileSize,
                    }).FirstOrDefault()
                }).ToListAsync();
        }

        public async Task<List<ResPostRankDTO>> GetNewPostByCategory(string category)
        {
            return await _context.BoardContents
                .Where(bc => bc.Category == category)
                .OrderByDescending(bc => bc.CreatedAt)
                .Take(3)
                .Select(bc => new ResPostRankDTO
                {
                    BoardContentId = bc.BoardContentId,
                    Category = bc.Category,
                    Title = bc.Title,
                    CreatedAt = bc.CreatedAt,
                    Writer = new ResWriterDTO
                    {
                        MemberId = bc.WriterId,
                        Nickname = bc.Writer.Nickname
                    },
                    Files = bc.BoardFiles
                    .OrderByDescending(bf => bf.BoardFileId)
                    .Take(1)
                    .Select(f => new ResBoardFileDTO
                    {
                        ThumbUrl = f.SaveFileName + f.Extension,
                        FileName = f.SaveFileName,
                        Extension = f.Extension,
                        FileSize = f.FileSize,
                    }).FirstOrDefault()
                }).ToListAsync();
        }

        public async Task<List<ResPostRankDTO>> GetTopFiveFreeBoards()
        {
            return await _context.BoardContents
                .Where(bc => bc.Category == "Board")
                .OrderByDescending(bc => bc.ViewCount)
                .Take(5)
                .Select(bc => new ResPostRankDTO
                {
                    BoardContentId = bc.BoardContentId,
                    Category = bc.Category,
                    Title = bc.Title,
                    CreatedAt = bc.CreatedAt,
                    Writer = new ResWriterDTO
                    {
                        MemberId = bc.WriterId,
                        Nickname = bc.Writer.Nickname
                    },
                }).ToListAsync(); ;
        }
    }


}
