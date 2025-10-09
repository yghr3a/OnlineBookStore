using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;
using System.Security.Claims;
using OnlineBookStore.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace OnlineBookStore.Services
{
    public class AccountService
    {
        private Respository<User> _responsity;
        private UserContext _userContext;

        public AccountService(Respository<User> repository, UserContext userContext)
        {
            _responsity = repository;
            _userContext = userContext;
        }

        public async Task<UserVerifyResult> VerifyLoggedInUserInformation(string userName, string password)
        {
            var quary = _responsity.AsQueryable();
            var user = await quary.FirstOrDefaultAsync(u => u.UserName == userName);

            if(user == null)
            {
                // 后续可以添加错误信息等
                return new UserVerifyResult { IsValid = false };
            }

            // TODO: 这里需要改为哈希密码比较
            // 这里简单比较密码
            if (user.PasswordHash != password)
            {
                // 后续可以添加错误信息等
                return new UserVerifyResult { IsValid = false };
            }

            var claims = new List<Claim>
            {
                // 这里传递的是用户编号,而非数据库内容使用的主键id
                new Claim(ClaimTypes.NameIdentifier, user.Number.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            return new UserVerifyResult
            {
                IsValid = true,
                ClaimList = claims
            };
        }
    }
}
