using System.IO;

namespace GowBoard.Models.DTO.RequestDTO
{
    public class ResFileResult
    {
        public Stream Resource { get; set; }
        public string FileName { get; set; }
        public int BoardFileId { get; set; }
    }
}