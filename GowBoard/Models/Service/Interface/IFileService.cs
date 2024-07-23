using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GowBoard.Models.Service.Interface
{
    public interface IFileService
    {

        Task<int> CreateFileAsync(HttpPostedFileBase file);
        Task<ResFileResult> DownloadFileAsync(int boardFileId);
        Task UpdateFileId(BoardFile boardFile);
        Task<bool> RemoveFileAsync(int boardFileId);
    }
}
