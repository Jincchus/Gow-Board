using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResDashboardDTO
    {
        public int MemberCount { get; set; }
        public int BoardCount { get; set; }
        public int NoticeCount { get; set; }
        public List<int> DailyMemberCount { get; set; }
        public List<int> DailyBoardCount { get; set; }
        public List<int> DailyNoticeCount { get; set; }
        public List<DateTime> Dates { get; set; }

        public List<string> FormattedDates
        {
            get
            {
                return Dates.Select(date => date.ToString("yyyy-MM-dd")).ToList();
            }
        }
    }
}