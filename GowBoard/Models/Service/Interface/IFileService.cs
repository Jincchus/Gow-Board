using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Entity;
using System.Threading.Tasks;
using System.Web;

namespace GowBoard.Models.Service.Interface
{
    public interface IFileService
    {

        Task<int> CreateFileAsync(HttpPostedFileBase file, bool isEditorImage);
        Task<ResFileResult> DownloadFileAsync(int boardFileId);
        Task UpdateFileId(BoardFile boardFile);
        Task<bool> RemoveFileAsync(int boardFileId);
        Task<BoardFile> GetFileByIdAsync(int boardFileId);
    }
}
