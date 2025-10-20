using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using OnlineBookStore.Services;

namespace OnlineBookStore.Pages.Account
{
    public class LoginModel : PageModel
    {
        // 登录页面的用户名和密码
        [BindProperty] public string UserName { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";

        // 显示在登录页面的内容盒下面的错误信息
        [BindProperty] public string? BoxDownErrorMessage { get; set; }

        // 负责用户登录相关业务操作的服务
        private AccountAppliaction _accountAppliaction;

        public LoginModel(AccountAppliaction accountAppliaction)
        {
            _accountAppliaction = accountAppliaction;
        }

        public void OnGet()
        {
        }

        /// <summary>
        /// 登录页面的Post操作
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // [2025/10/10] 添加用户名和密码皆为空的判断
            if (string.IsNullOrWhiteSpace(UserName) && string.IsNullOrWhiteSpace(Password))
            {
                BoxDownErrorMessage = "用户名和密码为空!";
                ModelState.AddModelError("", "用户名和密码为空");
                return Page();
            }

            // [2025/10/10] 添加用户名为空的判断
            if (string.IsNullOrWhiteSpace(UserName))
            {
                BoxDownErrorMessage = "用户名为空!";
                ModelState.AddModelError("", "用户名为空");
                return Page();
            }

            // [2025/10/10] 添加密码为空的判断
            if (string.IsNullOrWhiteSpace(Password))
            {
                BoxDownErrorMessage = "密码为空";
                ModelState.AddModelError("", "密码为空");
                return Page();
            }

            var userVerifyResult = await _accountAppliaction.VerifyLoggedInUserInformation(UserName, Password);

            if (userVerifyResult.IsValid == true)
            {
                // 身份声明
                var claimsIdentity = new ClaimsIdentity(userVerifyResult.ClaimList, CookieAuthenticationDefaults.AuthenticationScheme);
                
                // 认证属性
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // 是否保持登录
                    
                    // 7天后过期
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                // 具体的登录操作交给页面层处理而非服务层
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // 登录成功, 跳转到首页
                return RedirectToPage("/Index");
            }
            else
            {
                BoxDownErrorMessage = userVerifyResult.ErrorMsg;
                ModelState.AddModelError("", $"{userVerifyResult.ErrorMsg}");
                return Page();
            }

        }
    }
}
