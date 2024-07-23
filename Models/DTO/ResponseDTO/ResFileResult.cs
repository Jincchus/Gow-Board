using System.IO;

namespace GowBoard.Models.DTO.RequestDTO
{
    public class ResFileResult
    {
        public string Link { get; set; }
        public Stream Resource { get; set; }
        public string FileName { get; set; }
    }
}