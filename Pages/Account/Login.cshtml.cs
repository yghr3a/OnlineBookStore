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
        [BindProperty]
        public string UserName { get; set; } = "";
        [BindProperty]
        public string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }

        private AccountService _accountService;

        public LoginModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userVerifyResult = await _accountService.VerifyLoggedInUserInformation(UserName, Password);

            if (userVerifyResult.IsValid == true)
            {
                // 身份声明
                var claimsIdentity = new ClaimsIdentity(userVerifyResult.ClaimList, CookieAuthenticationDefaults.AuthenticationScheme);
                
                // 认证属性
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // 是否保持登录
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
                ErrorMessage = "用户名或密码错误";
                ModelState.AddModelError("", "用户名或密码错误");
                return Page();
            }

        }
    }
}
