using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GowBoard.Models.Service.Interface
{
    public interface IMemberService
    {
        RegisterResult RegisterMember(ReqRegisterDTO registerDto);

        RegisterResult DuplicatedCheckId(string memberId);

        RegisterResult DuplicatedCheckNickname(string nickname);

        RegisterResult DuplicatedCheckEmail(string nickname);

        Tuple<bool, string> SendAuthenticationEmail(string email);

        LoginResult Login(ReqLoginDTO loginDto);

        Member GetMemberById(string memberId);

        bool VerifyCredentials(string memberId, string password);
        void DeleteMember(string memberId);

        UpdateProfileResult UpdateMemberProfile(Member originMemberInfo, ReqUpdateProfileDTO reqUpdateProfileDto);

        Role GetRoleByMemberId(string memberId);
        Task<int> GetTotalMemberCountAsync();
        Task<(List<DateTime>, List<int>)> GetDailyMemberCountAsync(int days);
        Task<List<ResAdminMemberListDTO>> GetMemberList();
    }
}
