namespace GowBoard.Models.DTO.RequestDTO
{
    public class ReqSearchBoardDTO
    {
        public string Category { get; set; }
        public int Page = 1;
        public int PageSize = 10;
        public string SearchType { get; set; }
        public string SearchKeyword { get; set; }
    }
}