using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineBookStore.Pages._TestPage
{
    public class _testemailsendModel : PageModel
    {
        public EmailSendService _emailSendService;
        public EmailVerificationTokenService _emailVerificationTokenService;

        public _testemailsendModel(EmailSendService emailSendService, EmailVerificationTokenService emailVerificationService)
        {
            _emailSendService = emailSendService;
            _emailVerificationTokenService = emailVerificationService;
        }

        public void OnGet()
        {

        }

        public async Task OnPostAsync()
        {
            var token = _emailVerificationTokenService.GenerateToken("1764016008@qq.com");
            var verificationUrl = $"https://localhost:7109/api/account/verify-email?token={token}";

            var subject = "验证您的邮箱";
            var body = $@"
        <h2>欢迎使用本站！</h2>
        <p>请点击以下链接以验证您的邮箱：</p>
        <a href='{verificationUrl}'>立即验证</a>
        <p>该链接2小时内有效。</p>";

            await _emailSendService.SendAsync("1764016008@qq.com", subject, body);
        }
    }
}
