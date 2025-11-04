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
        private EmailVerificationTokenService _emailVerificationTokenService;
        private EmailSendService _emailSendService;
        private UserDomainService _userDomainService;
        private UserFactory _userFactory;
        private UrlFactory _urlFactory;

        public AccountAppliaction(EmailVerificationTokenService emailVerificationTokenService,
                                  EmailSendService emailSendService,
                                  UserDomainService userDomainService,
                                  UserFactory userFactory,
                                  UrlFactory urlFactory)
        {
            _emailVerificationTokenService = emailVerificationTokenService;
            _emailSendService = emailSendService;
            _userDomainService = userDomainService;
            _userFactory = userFactory;
            _urlFactory = urlFactory;
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
        public async Task<InfoResult> UserRegistraionAsync(UserRegisterInfo info)
        {
            try
            {
                // 验证用户登录信息唯一性
                await CheckAsync(_userDomainService.VerifyUserResiterInfo(info));
                // 创建新用户
                var newUser = Check(_userFactory.CreateUserEntity(info));
                // 将还没做信息验证的用户先添加到数据库里
                await CheckAsync(_userDomainService.AddUser(newUser));
                // 生成验证用的Token
                string token = Check(_emailVerificationTokenService.GenerateToken(info.Email));

                // 在这里构建具体的验证url, 以后再重构 
                var verificationUrl = _urlFactory.CreateTokenUrl("api/account/verify/email", token);

                // 这里先在这里定义邮件的格式, 后面再重构
                var subject = "验证您的邮箱";
                var body = $@"
                <h2>欢迎使用本站！</h2>
                <p>请点击以下链接以验证您的邮箱：</p>
                <a href='{verificationUrl}'>立即验证</a>
                <p>该链接2小时内有效。</p>";

                // 发送Token
                await _emailSendService.SendAsync(info.Email, subject, body);

                return InfoResult.Success();

            }
            catch (Exception ex)
            {
                return InfoResult.Fail("注册失败" + ex.Message  );
            }
        }

        /// <summary>
        /// 验证注册Token        /// </summary>
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
