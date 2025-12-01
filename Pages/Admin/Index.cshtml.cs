using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineBookStore.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

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
