using GowBoard.Models.Context;
using GowBoard.Models.DTO.RequestDTO;
using GowBoard.Models.DTO.ResponseDTO;
using GowBoard.Models.Entity;
using GowBoard.Models.Service.Interface;
using GowBoard.Models.Service.Utility;
using GowBoard.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;


namespace GowBoard.Models.Service
{
    public class MemberService : IMemberService
    {
        private readonly GowBoardContext _context;
        private readonly PasswordHash _passwordHash;

        public MemberService(GowBoardContext context)
        {
            _context = context;
            _passwordHash = new PasswordHash();
        }


        public RegisterResult RegisterMember(ReqRegisterDTO registerDto)
        {
            var result = new RegisterResult
            {
                Success = false,
                Message = "회원가입에 실패하였습니다. 다시 시도하여주십시오"
            };


            if (string.IsNullOrEmpty(registerDto.Memberid)
                || string.IsNullOrEmpty(registerDto.Name)
                || string.IsNullOrEmpty(registerDto.Nickname)
                || string.IsNullOrEmpty(registerDto.Email)
                || string.IsNullOrEmpty(registerDto.Password))
            {
                result.Message = "빈 값은 입력 될 수 없습니다. 입력된 값을 확인해 주세요.";
                return result;
            }


            var memberContext = _context.Members;
            if (memberContext.Any(m => m.MemberId.Equals(registerDto.Memberid) || m.Nickname.Equals(registerDto.Nickname)))
            {
                result.Message = "아이디 혹은 닉네임 중복 확인이 필요합니다.";
                return result;
            }
            if (memberContext.Any(m => m.Email == registerDto.Email))
            {
                result.Message = "중복된 이메일입니다.";
                return result;
            }

            try
            {

                DateTime koreaNow = DateTimeUtility.GetKoreanNow();
                string hashedPassword = _passwordHash.HashPassword(registerDto.Password);

                var member = new Member
                {
                    MemberId = registerDto.Memberid,
                    Password = hashedPassword,
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Nickname = registerDto.Nickname,
                    Phone = registerDto.Phone,
                    CreatedAt = koreaNow
                };

                _context.Members.Add(member);
                _context.SaveChanges();

                var memberRole = _context.Roles.FirstOrDefault(r => r.RoleName == "member");
                if (memberRole == null)
                {
                    _context.Roles.Add(memberRole);
                    _context.SaveChanges();
                }

                var memberRoleMap = new MemberRoleMap
                {
                    MemberId = member.MemberId,
                    RoleId = memberRole.RoleId
                };
                _context.MemberRoleMaps.Add(memberRoleMap);
                _context.SaveChanges();

                result.Success = true;
                result.Message = "회원가입에 성공하였습니다";

                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

        }

        public RegisterResult DuplicatedCheckId(string memberId)
        {
            var result = new RegisterResult
            {
                Success = false,
                Message = "회원가입에 실패하였습니다. 다시 시도하여주십시오"
            };

            var memberContext = _context.Members;
            if (memberContext.Any(m => m.MemberId == memberId))
            {
                result.Message = "이미 존재하는 아이디입니다.";
                return result;
            }

            result.Success = true;
            result.Message = "사용 가능한 아이디입니다";

            return result;
        }

        public RegisterResult DuplicatedCheckNickname(string nickname)
        {
            var result = new RegisterResult
            {
                Success = false,
                Message = "회원가입에 실패하였습니다. 다시 시도하여주십시오"
            };

            var memberContext = _context.Members;
            if (memberContext.Any(m => m.Nickname == nickname))
            {
                result.Message = "이미 존재하는 닉네임입니다.";
                return result;
            }

            result.Success = true;
            result.Message = "사용 가능한 닉네임 입니다";

            return result;
        }
        public RegisterResult DuplicatedCheckEmail(string email)
        {
            var result = new RegisterResult
            {
                Success = false,
                Message = "회원가입에 실패하였습니다. 다시 시도하여주십시오"
            };

            var memberContext = _context.Members;
            if (memberContext.Any(m => m.Email == email))
            {
                result.Message = "이미 존재하는 이메일입니다.";
                return result;
            }

            result.Success = true;
            result.Message = "사용 가능한 이메일 입니다";

            return result;
        }

        public Tuple<bool, string> SendAuthenticationEmail(string email)
        {
            string authNumber = AuthNumberGenerator.GenerateAuthNumber();

            string subject = "GowBoard 회원가입 인증번호입니다.";
            string body = $@"
                    <p>GowBoard 회원가입을 위한 인증번호를 요청하셨습니다</p>
                    <p>인증번호는 <strong>{authNumber}</strong></p>
                    <p>해당 인증번호를 입력하여 회원가입을 진행하여주십시오</p>
                    <p>감사합니다</p>";
            bool emailSent = EmailUtility.SendEmail(email, subject, body);

            return Tuple.Create(emailSent, authNumber);
        }

        public LoginResult Login(ReqLoginDTO loginDto)
        {
            var member = _context.Members.FirstOrDefault(m => m.MemberId.Equals(loginDto.MemberId));

            if (member == null)
            {
                return new LoginResult { IsSuccess = false };
            }
            if (member.DeleteYn)
            {
                return new LoginResult { IsSuccess = false, IsDeleted = true };
            }

            string hashedPassword = _passwordHash.HashPassword(loginDto.Password);
            if (!member.Password.Equals(hashedPassword))
            {
                return new LoginResult { IsSuccess = false };
            }

            return new LoginResult { IsSuccess = true, Member = member };
        }

        public Member GetMemberById(string memberId)
        {
            return _context.Members.FirstOrDefault(m => m.MemberId.Equals(memberId));
        }


        // 회원 정보 조회
        public bool VerifyCredentials(string memberId, string password)
        {
            string hashedPassword = _passwordHash.HashPassword(password);
            var member = _context.Members.FirstOrDefault(m => m.MemberId.Equals(memberId) && m.Password.Equals(hashedPassword));

            return member != null && !member.DeleteYn;
        }

        // 회원 정보 업데이트
        public UpdateProfileResult UpdateMemberProfile(Member originMemberInfo, ReqUpdateProfileDTO reqUpdateProfileDto)
        {

            var result = new UpdateProfileResult
            {
                Success = true,
                Message = "회원정보가 수정되었습니다"
            };

            bool isChanges = false;

            if (!string.IsNullOrEmpty(reqUpdateProfileDto.OriginPassword))
            {
                string originHashedPassword = _passwordHash.HashPassword(reqUpdateProfileDto.OriginPassword);
                if (!originMemberInfo.Password.Equals(originHashedPassword))
                {
                    result.Success = false;
                    result.Message = "현재 비밀번호가 일치하지 않습니다.";
                    return result;
                }
            }

            if (!string.IsNullOrEmpty(reqUpdateProfileDto.Password))
            {

                string newHashedPassword = _passwordHash.HashPassword(reqUpdateProfileDto.Password);

                if (originMemberInfo.Password != newHashedPassword)
                {
                    originMemberInfo.Password = newHashedPassword;
                    isChanges = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "변경 할 비밀번호와 원래 비밀번호가 같습니다.";
                }
            }

            if (!string.IsNullOrEmpty(reqUpdateProfileDto.Email) && !string.Equals(reqUpdateProfileDto.Email, originMemberInfo.Email))
            {
                originMemberInfo.Email = reqUpdateProfileDto.Email;
                isChanges = true;
            }

            if (!string.IsNullOrEmpty(reqUpdateProfileDto.Nickname) && !string.Equals(reqUpdateProfileDto.Nickname, originMemberInfo.Nickname))
            {
                originMemberInfo.Nickname = reqUpdateProfileDto.Nickname;
                isChanges = true;
            }

            if (!string.IsNullOrEmpty(reqUpdateProfileDto.Phone) && !string.Equals(reqUpdateProfileDto.Phone, originMemberInfo.Phone))
            {
                originMemberInfo.Phone = reqUpdateProfileDto.Phone;
                isChanges = true;
            }

            if (!isChanges)
            {
                result.Success = false;
                result.Message = "변경된 값이 존재하지 않습니다.";
                return result;
            }


            _context.Entry(originMemberInfo).State = EntityState.Modified;
            _context.SaveChanges();


            return result;
        }

        // 회원 탈퇴
        public void DeleteMember(string memberId)
        {
            DateTime koreaNow = DateTimeUtility.GetKoreanNow();

            var member = _context.Members.FirstOrDefault(m => m.MemberId.Equals(memberId));
            if (member != null)
            {
                member.DeleteYn = true;
                member.DeletedAt = koreaNow;
                _context.SaveChanges();
            }
        }

        public Role GetRoleByMemberId(string memberId)
        {
            var role = _context.MemberRoleMaps
                .Include(mrm => mrm.Role)
                .FirstOrDefault(mrm => mrm.MemberId == memberId)?.Role;
            return role ?? new Role { RoleName = "member" };
        }

        public async Task<int> GetTotalMemberCountAsync()
        {
            var memberCount = await _context.Members.CountAsync();

            return memberCount;
        }

        public async Task<(List<DateTime>, List<int>)> GetDailyMemberCountAsync(int days)
        {
            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddDays(-days + 1);

            var dailyCounts = await _context.Members
                .Where(m => DbFunctions.TruncateTime(m.CreatedAt) >= startDate && DbFunctions.TruncateTime(m.CreatedAt) <= endDate)
                .GroupBy(m => DbFunctions.TruncateTime(m.CreatedAt))
                .Select(g => new { Date = g.Key.Value, Count = g.Count() }) 
                .OrderBy(g => g.Date)
                .ToListAsync();

            var dates = Enumerable.Range(0, days)
                .Select(i => startDate.AddDays(i))
                .ToList();

            var counts = dates.Select(date => dailyCounts.FirstOrDefault(d => d.Date == date)?.Count ?? 0).ToList();

            return (dates, counts);
        }

        public async Task<List<ResAdminMemberListDTO>> GetMemberList(int page, int pageSize)
        {
            var query = from m in _context.Members
                        join mrm in _context.MemberRoleMaps on m.MemberId equals mrm.MemberId
                        join r in _context.Roles on mrm.RoleId equals r.RoleId
                        select new ResAdminMemberListDTO
                        {
                            MemberId = m.MemberId,
                            Name = m.Name,
                            Email = m.Email,
                            Nickname = m.Nickname,
                            Phone = m.Phone,
                            DeleteYn = m.DeleteYn,
                            DeletedAt = m.DeletedAt,
                            RoleName = r.RoleName,
                        };

            return await query
                         .OrderBy(m => m.MemberId)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
        }

        public async Task<int> GetTotalMemberCount()
        {
            return await _context.Members.CountAsync();
        }
        /*
        public async Task<List<ResAdminMemberListDTO>> GetMemberList()
        {
            var query = from m in _context.Members
                        join mrm in _context.MemberRoleMaps on m.MemberId equals mrm.MemberId
                        join r in _context.Roles on mrm.RoleId equals r.RoleId
                        select new ResAdminMemberListDTO
                        {
                            MemberId = m.MemberId,
                            Name = m.Name,
                            Email = m.Email,
                            Nickname = m.Nickname,
                            Phone = m.Phone,
                            DeleteYn = m.DeleteYn,
                            DeletedAt = m.DeletedAt,
                            RoleName = r.RoleName,
                        };

            return await query.ToListAsync();
        }*/
    }

}