using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace OnlineBookStore.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string UserName { get; set; } = "";
        [BindProperty]
        public string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // ����ģ���¼��֤�����滻Ϊ���ݿ�У��
            if (UserName == "test" && Password == "123456")
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.Email, "test@example.com")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // �Ƿ񱣳ֵ�¼
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", "�û������������");
            return Page();
        }
    }
}
