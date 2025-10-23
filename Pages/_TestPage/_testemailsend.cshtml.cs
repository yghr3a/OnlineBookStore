using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineBookStore.Pages._TestPage
{
    public class _testemailsendModel : PageModel
    {
        public EmailSendService _emailSendService;

        public _testemailsendModel(EmailSendService emailSendService)
        {
            _emailSendService = emailSendService;
        }

        public void OnGet()
        {

        }

        public async Task OnPostAsync()
        {
            await _emailSendService.SendAsync("1764016008@qq.com", "测试邮件",
    "<p>你好，这是一封测试邮件。</p>");
        }
    }
}
