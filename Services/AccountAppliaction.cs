using Microsoft.AspNetCore.Identity;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Respository;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 账户相关业务操作
    /// </summary>
    public class AccountAppliaction
    {
        private Respository<User> _responsity;
        private UserContext _userContext;
        private IPasswordHasher<User> _passwordHasher;

        public AccountAppliaction(Respository<User> repository, UserContext userContext, IPasswordHasher<User> passwordHasher)
        {
            _responsity = repository;
            _userContext = userContext;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// 验证用户登录信息业务操作
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<UserVerifyResult> VerifyLoggedInUserInformation(string userName, string password)
        {
            var quary = _responsity.AsQueryable();
            var user = await quary.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                // 后续可以添加错误信息等
                // [2025/10/10]后续错误信息可以改为自定义业务异常
                return new UserVerifyResult
                {
                    IsValid = false,
                    ErrorMsg = "该用户不存在"
                };
            }

            // 验证密码, 这里使用ASP.NET Core Identity的密码哈希验证
            var pwHashVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            // 如果验证失败
            if (pwHashVerificationResult == PasswordVerificationResult.Failed)
            {
                // 后续可以添加错误信息等
                // [2025/10/10]后续错误信息可以改为自定义业务异常
                return new UserVerifyResult
                {
                    IsValid = false,
                    ErrorMsg = "密码错误"
                };
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

        /// <summary>
        /// 用户注册业务操作
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<UserRegisterResult> UserRegistraionAsync(string userName, string password, string email)
        {
            var quary = _responsity.AsQueryable();
            var passwordHash = _passwordHasher.HashPassword(null, password);

            // 检查用户名和邮箱的唯一性
            var existingUser = await quary.FirstOrDefaultAsync(u => u.UserName == userName);
            var existingEmail = await quary.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                return new UserRegisterResult
                {
                    IsSuccess = false,
                    ErrorMsg = "用户名已存在"
                };
            }

            if (existingEmail != null)
            {
                return new UserRegisterResult
                {
                    IsSuccess = false,
                    ErrorMsg = "该邮箱已被注册"
                };
            }

            // TODO: 增加邮箱或者手机号码的唯一性验证

            // 创建新用户的实体模型类对象
            var newUser = new User
            {
                // Number字段后续需要改为更合理的生成方式, 现在只是简单的用时间戳
                Number = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % int.MaxValue),
                UserName = userName,
                PasswordHash = passwordHash,
                Email = email,
                UserRole = UserRole.Customer,
                RegistrationDate = DateTime.UtcNow
            };

            // 添加新用户到数据库
            // [2025/10/11] 此时确保UserID生成
            await _responsity.AddAsync(newUser);
            await _responsity.SaveAsync();

            // [2025/10/11]  尝试手动创建一个Cart对象赋值给User, 看看User会不会带有Cast引导属性
            var cart = new Cart()
            {
                User = newUser
            };

            newUser.Cart = cart;

            // 再次保存
            await _responsity.SaveAsync();

            // 创建用户声明列表, 用于注册完成后自动登录
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Number.ToString()),
                new Claim(ClaimTypes.Name, newUser.UserName),
                new Claim(ClaimTypes.Email, newUser.Email ?? "")
            };

            return new UserRegisterResult
            {
                IsSuccess = true,
                ClaimList = claims
            };

        }

        //public async Task<InfoResult> VerifyUserRegisterTokenAsync(string token)
        //{
        //    var email = _tokenService.ValidateToken(token);
        //    if (email == null)
        //        return BadRequest("验证链接无效或已过期。");

        //    // 找到用户并更新状态
        //    var userRes = await _userDomainService.GetUserByEmailAsync(email);
        //    if (userRes.IsSuccess == false)
        //        return BadRequest("用户不存在。");

        //    var user = userRes.Data!;
        //    //user.IsEmailVerified = true;
        //    //await _userDomainService.UpdateAsync(user);

        //    return Ok("邮箱验证成功！");
        //}
    }
}
