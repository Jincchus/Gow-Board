using System.IO;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResFileResult
    {
        public Stream Resource { get; set; }
        public string FileName { get; set; }
        public int BoardFileId { get; set; }
        public bool IsEditorImage { get; set; }
    }
}