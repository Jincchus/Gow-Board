using System;

namespace GowBoard.Models.DTO.ResponseDTO
{
    public class ResAdminMemberListDTO
    {
        public string MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Phone { get; set; }
        public bool DeleteYn { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string RoleName { get; set; }


        public string FormatterDates
        {
            get
            {
                return DeletedAt.HasValue ? DeletedAt.Value.ToString("yyyy.MM.dd tt hh:mm") : "-";
            }
        }
    }
}