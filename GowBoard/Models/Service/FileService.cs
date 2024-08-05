using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GowBoard.Models.Context;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Entity;
using GowBoard.Models.Service.Interface;
using GowBoard.Utility;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GowBoard.Models.Service
{
    public class FileService : IFileService
    {
        private readonly GowBoardContext _context;
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly string _thumbnailContainerName;

        public FileService(GowBoardContext context)
        {
            _context = context;

            _connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString), "The connection string is null or empty. Please check your Web.config file.");
            }

            _containerName = ConfigurationManager.AppSettings["AzureStorageContainerName"];
            if (string.IsNullOrEmpty(_containerName))
            {
                throw new ArgumentNullException(nameof(_containerName), "The container name is null or empty. Please check your Web.config file.");
            }

            _thumbnailContainerName = ConfigurationManager.AppSettings["AzureStorageThumbnailContainerName"];
            if (string.IsNullOrEmpty(_thumbnailContainerName))
            {
                throw new ArgumentNullException(nameof(_thumbnailContainerName), "The thumbnail container name is null or empty.");
            }
        }
        public async Task<int> CreateFileAsync(HttpPostedFileBase file, bool isEditorImage)
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("파일이 존재하지 않습니다");

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var thumbnailContainerClient = blobServiceClient.GetBlobContainerClient(_thumbnailContainerName);

            await containerClient.CreateIfNotExistsAsync();
            await thumbnailContainerClient.CreateIfNotExistsAsync();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    DateTime koreaNow = DateTimeUtility.GetKoreanNow();
                    string originFileName = Path.GetFileName(file.FileName);
                    string saveFileName = Guid.NewGuid().ToString();
                    int fileSize = file.ContentLength;
                    string extension = Path.GetExtension(file.FileName);

                    var boardFile = new BoardFile
                    {
                        FileSize = fileSize,
                        Extension = extension,
                        OriginFileName = originFileName,
                        SaveFileName = saveFileName,
                        CreatedAt = koreaNow,
                        IsEditorImage = isEditorImage
                    };

                    _context.BoardFiles.Add(boardFile);
                    await _context.SaveChangesAsync();

                    int boardFileId = boardFile.BoardFileId;

                    var blobClient = containerClient.GetBlobClient(saveFileName + extension);
                    await blobClient.UploadAsync(file.InputStream, new BlobHttpHeaders { ContentType = file.ContentType });

                    if (IsImageFile(extension))
                    {
                        var thumbnailBlobClient = thumbnailContainerClient.GetBlobClient(saveFileName + "Thumb" + extension);

                        using (var memoryStream = new MemoryStream())
                        {
                            file.InputStream.Seek(0, SeekOrigin.Begin);
                            file.InputStream.CopyTo(memoryStream);
                            memoryStream.Position = 0;

                            try
                            {
                                using (var image = Image.FromStream(memoryStream))
                                using (var thumbnail = new Bitmap(100, 100))
                                using (var graphics = Graphics.FromImage(thumbnail))
                                {
                                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                                    graphics.DrawImage(image, 0, 0, 100, 100);

                                    using (var thumbnailStream = new MemoryStream())
                                    {
                                        thumbnail.Save(thumbnailStream, image.RawFormat);
                                        thumbnailStream.Position = 0;
                                        await thumbnailBlobClient.UploadAsync(thumbnailStream, new BlobHttpHeaders { ContentType = file.ContentType });
                                    }
                                }
                            }
                            catch (ArgumentException ex)
                            {
                                throw new ApplicationException("유효하지 않은 이미지 파일입니다.", ex);
                            }
                        }
                    }

                    transaction.Commit();
                    return boardFileId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException("파일 업로드 중 오류가 발생했습니다.", ex);
                }
            }
        }

        private bool IsImageFile(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
            return imageExtensions.Contains(extension.ToLower());
        }


        public async Task<ResFileResult> DownloadFileAsync(int boardFileId)
        {
            var file = await GetFileByIdAsync(boardFileId);

            if (file == null)
                return null;

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(file.SaveFileName + file.Extension);

            if (!await blobClient.ExistsAsync())
                return null;

            var blobDownloadInfo = await blobClient.DownloadAsync();

            return new ResFileResult
            {
                FileName = file.OriginFileName,
                Resource = blobDownloadInfo.Value.Content
            };
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

        public async Task<bool> RemoveFileAsync(int boardFileId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var file = await _context.BoardFiles.FindAsync(boardFileId);
                    if (file == null) return false;

                    var blobServiceClient = new BlobServiceClient(_connectionString);
                    var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                    var blobClient = containerClient.GetBlobClient(file.SaveFileName + file.Extension);
                    await blobClient.DeleteIfExistsAsync();

                    var thumbBlobClient = containerClient.GetBlobClient(file.SaveFileName + "Thumb" + file.Extension);
                    await thumbBlobClient.DeleteIfExistsAsync();

                    _context.BoardFiles.Remove(file);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException("파일 삭제 중 오류가 발생했습니다.", ex);
                }
            }
        }
    }
}