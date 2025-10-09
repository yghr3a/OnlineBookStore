using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;
using System.Security.Claims;
using OnlineBookStore.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace OnlineBookStore.Services
{
    public class AccountService
    {
        private Respository<User> _responsity;
        private UserContext _userContext;
        private IPasswordHasher<User> _passwordHasher;

        public AccountService(Respository<User> repository, UserContext userContext, IPasswordHasher<User> passwordHasher)
        {
            _responsity = repository;
            _userContext = userContext;
            _passwordHasher = passwordHasher;
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

            // 验证密码, 这里使用ASP.NET Core Identity的密码哈希验证
            var pwHashVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            // 如果验证失败
            if (pwHashVerificationResult == PasswordVerificationResult.Failed)
            {
                // 后续可以添加错误信息等
                return new UserVerifyResult { IsValid = false };
            }

            var claims = new List<Claim>
            {
                // 这里传递的是用户编号,而非数据库内容使用的主键id
                new Claim(ClaimTypes.NameIdentifier, user.Number.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            return new UserVerifyResult
            {
                IsValid = true,
                ClaimList = claims
            };
        }
    }
}
