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
        // ��¼ҳ����û���������
        [BindProperty] public string UserName { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";

        // ��ʾ�ڵ�¼ҳ������ݺ�����Ĵ�����Ϣ
        [BindProperty] public string? BoxDownErrorMessage { get; set; }

        // �����û���¼���ҵ������ķ���
        private AccountAppliaction _accountAppliaction;

        public LoginModel(AccountAppliaction accountAppliaction)
        {
            _accountAppliaction = accountAppliaction;
        }

        public void OnGet()
        {
        }

        /// <summary>
        /// ��¼ҳ���Post����
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // [2025/10/10] ����û����������Ϊ�յ��ж�
            if (string.IsNullOrWhiteSpace(UserName) && string.IsNullOrWhiteSpace(Password))
            {
                BoxDownErrorMessage = "�û���������Ϊ��!";
                ModelState.AddModelError("", "�û���������Ϊ��");
                return Page();
            }

            // [2025/10/10] ����û���Ϊ�յ��ж�
            if (string.IsNullOrWhiteSpace(UserName))
            {
                BoxDownErrorMessage = "�û���Ϊ��!";
                ModelState.AddModelError("", "�û���Ϊ��");
                return Page();
            }

            // [2025/10/10] �������Ϊ�յ��ж�
            if (string.IsNullOrWhiteSpace(Password))
            {
                BoxDownErrorMessage = "����Ϊ��";
                ModelState.AddModelError("", "����Ϊ��");
                return Page();
            }

            var userVerifyResult = await _accountAppliaction.VerifyLoggedInUserInformation(UserName, Password);

            if (userVerifyResult.IsValid == true)
            {
                // �������
                var claimsIdentity = new ClaimsIdentity(userVerifyResult.ClaimList, CookieAuthenticationDefaults.AuthenticationScheme);
                
                // ��֤����
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // �Ƿ񱣳ֵ�¼
                    
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
