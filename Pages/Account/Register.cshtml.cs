using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Services;
using System.Security.Claims;

namespace OnlineBookStore.Pages.Account
{
    public class RegisterModel : PageModel
    {
        // ע��ҳ����û���, ����, ����, ȷ������
        [BindProperty] public string UserName { get; set; } = "";
        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        [BindProperty] public string ConfirmPassword { get; set; } = "";

        // ��ʾ��ע��ҳ������ݺ�����Ĵ�����Ϣ
        [BindProperty] public string? BoxDownErrorMessage { get; set; }

        // �����û�ע�����ҵ������ķ���
        private AccountService _accountService;

        public RegisterModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            // �û���Ϊ�յ��ж�
            if (string.IsNullOrWhiteSpace(UserName))
            {
                BoxDownErrorMessage = "�û���Ϊ��!";
                ModelState.AddModelError("", "�û���Ϊ��");
                return Page();
            }

            // ����Ϊ�յ��ж�
            if (string.IsNullOrWhiteSpace(Email))
            {
                BoxDownErrorMessage = "����Ϊ��!";
                ModelState.AddModelError("", "����Ϊ��");
                return Page();
            }

            // ����Ϊ�յ��ж�
            if (string.IsNullOrWhiteSpace(Password))
            {
                BoxDownErrorMessage = "����Ϊ��";
                ModelState.AddModelError("", "����Ϊ��");
                return Page();
            }

            // ȷ������Ϊ�յ��ж�
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                BoxDownErrorMessage = "ȷ������Ϊ��";
                ModelState.AddModelError("", "ȷ������Ϊ��");
                return Page();
            }

            // �����ȷ�����벻ƥ����ж�
            if (Password != ConfirmPassword)
            {
                BoxDownErrorMessage = "�����ȷ�����벻ƥ��";
                ModelState.AddModelError("", "�����ȷ�����벻ƥ��");
                return Page();
            }

            var userRegisterResult = await _accountService.UserRegistraionAsync(UserName, Password, Email);

            if (userRegisterResult.IsSuccess == true)
            {
                // ע��ɹ�, �Զ���¼

                var claimsIdentity = new ClaimsIdentity(userRegisterResult.ClaimList, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    // ��¼��������Ч
                    IsPersistent = true,
                    // 7������
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                // ����ĵ�¼��������ҳ��㴦����Ƿ����
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // ��¼�ɹ�, ��ת����ҳ
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "ע��ɹ���",
                    RedirectUrl = "/Index",
                    Seconds = 5
                });
            }
            else
            {
                BoxDownErrorMessage = userRegisterResult.ErrorMsg;
                ModelState.AddModelError("", $"{userRegisterResult.ErrorMsg}");
                return Page();
            }

        }
    }
}
