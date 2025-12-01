using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Services;
using Microsoft.AspNetCore.Authorization;

namespace OnlineBookStore.Pages.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private UserContext userContext;
        public string UserNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public IndexModel(UserContext userContext)
        {
            this.userContext = userContext;
        }

        public void OnGet()
        {
            Email = userContext.Email!;
            UserName = userContext.UserName!;
            UserNumber = userContext.UserId!;
            Role = userContext.RawRoleString!;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Shared/Notification", new
            {
                Message = "退出登录成功,返回登录页面",
                RedirectUrl = "/Account/Login",
                Seconds = 3
            });
        }
    }
}
