using Microsoft.AspNetCore.Identity;
using OnlineBookStore.Models.Data;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Infrastructure;
using static OnlineBookStore.Infrastructure.ExceptionChecker;

namespace OnlineBookStore.Services
{
    /// <summary>
    /// 账户相关业务操作
    /// </summary>
    public class AccountAppliaction
    {
        private Repository<User> _responsity;
        private UserContext _userContext;
        private EmailVerificationTokenService _emailVerificationTokenService;
        private UserDomainService _userDomainService;
        private IPasswordHasher<User> _passwordHasher;

        public AccountAppliaction(Repository<User> repository,
                                       UserContext userContext, 
                                       IPasswordHasher<User> passwordHasher,
                                       EmailVerificationTokenService emailVerificationTokenService,
                                       UserDomainService userDomainService)
        {
            _responsity = repository;
            _userContext = userContext;
            _passwordHasher = passwordHasher;
            _emailVerificationTokenService = emailVerificationTokenService;
            _userDomainService = userDomainService;
        }

        /// <summary>
        /// 验证用户登录信息业务操作
        /// </summary>
        /// [2025/11/03] 使用ExceptionChecker简化代码
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<UserVerifyResult> VerifyLoggedInUserInformation(string userName, string password)
        {
            try
            {
                var user = await CheckAsync(_userDomainService.GetUserByUserNameAsync(userName));
                Check(_userDomainService.VerifyUserPassword(user!, password));

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
            catch (Exception ex)
            {
                return new UserVerifyResult
                {
                    IsValid = false,
                    ErrorMsg = $"{ex.Message}"
                };
            }
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

        /// <summary>
        /// 验证注册Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<InfoResult> VerifyUserRegisterTokenAsync(string token)
        {
            try
            {
                // 验证token并返回邮箱信息
                var email = Check(_emailVerificationTokenService.ValidateTokenAndReturnEmail(token));

                // 找到用户并更新状态
                var user = await CheckAsync(_userDomainService.GetUserByEmailAsync(email));

                // 更新用户验证状态
                user.IsEmailVerified = true;
                await CheckAsync(_userDomainService.UpdateAsync(user));

                return InfoResult.Success();
            }
            catch (Exception ex)
            {
                // TODO: 后续添加业务异常处理
                return InfoResult.Fail($"验证Token失败 " + ex.Message);
            }
        }
    }
}
