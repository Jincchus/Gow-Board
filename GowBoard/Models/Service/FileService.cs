using GowBoard.Models.Context;
using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.Entity;
using GowBoard.Models.Service.Interface;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GowBoard.Models.Service
{
    public class FileService : IFileService
    {
        private readonly GowBoardContext _context;

        public FileService(GowBoardContext context)
        {
            _context = context;
        }

        public async Task<int> CreateFileAsync(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("파일이 존재하지 않습니다");

            int boardFileId = await GetSequenceAsync();
            string originFileName = Path.GetFileName(file.FileName);
            string saveFileName = Guid.NewGuid().ToString();
            int fileSize = file.ContentLength;
            string extension = Path.GetExtension(file.FileName);

            var boardFile = new BoardFile
            {
                BoardFileId = boardFileId,
                FileSize = fileSize,
                Extension = extension,
                OriginFileName = originFileName,
                SaveFileName = saveFileName,
                CreatedAt = DateTime.Now,
            };

            await AddFileAsync(boardFile);

            string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), saveFileName+extension);
            file.SaveAs(filePath);

            string thumbFileName = saveFileName + "Thumb" + extension;
            string thumbFilePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/Thumbnail"), thumbFileName);
            var thumbnail = Image.FromFile(filePath).GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
            thumbnail.Save(thumbFilePath);

            return boardFileId;
        }

        public async Task<int> GetSequenceAsync()
        {
            var lastFile = await _context.BoardFiles.OrderByDescending(f => f.BoardFileId).FirstOrDefaultAsync();
            return (lastFile?.BoardFileId ?? 0) + 1;
        }

        public async Task AddFileAsync(BoardFile file)
        {
            _context.BoardFiles.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task<ResFileResult> DownloadFileAsync(int boardFileId)
        {
            BoardFile file = await GetFileByIdAsync(boardFileId);

            if (file == null)
            {
                return null;
            }

            string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), file.SaveFileName+file.Extension);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                var resource = new MemoryStream(fileBytes);

                return new ResFileResult
                {
                    FileName = file.OriginFileName,
                    Resource = resource
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<BoardFile> GetFileByIdAsync(int boardFileId)
        {
            return await _context.BoardFiles.FirstOrDefaultAsync(f => f.BoardFileId == boardFileId);
        }

        public async Task UpdateFileId(BoardFile boardFile)
        {
            var existingFile = await _context.BoardFiles.FindAsync(boardFile.BoardFileId);
            if (existingFile != null)
            {
                existingFile.BoardContentId = boardFile.BoardContentId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RemoveFile(int boardFileId)
        {
            var file = await _context.BoardFiles.FindAsync(boardFileId);
            if (file == null) return false;

            _context.BoardFiles.Remove(file);
            await _context.SaveChangesAsync();

            string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads"), file.SaveFileName + file.Extension);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return true;
        }






    }
}