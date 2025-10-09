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
                // �������
                var claimsIdentity = new ClaimsIdentity(userVerifyResult.ClaimList, CookieAuthenticationDefaults.AuthenticationScheme);
                
                // ��֤����
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // �Ƿ񱣳ֵ�¼
                };

                // ����ĵ�¼��������ҳ��㴦����Ƿ����
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // ��¼�ɹ�, ��ת����ҳ
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "�û������������";
                ModelState.AddModelError("", "�û������������");
                return Page();
            }

        }
    }
}
