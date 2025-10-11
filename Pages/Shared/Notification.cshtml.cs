using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineBookStore.Pages.Shared
{
    public class NotificationModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Message { get; set; } = "操作成功";

        [BindProperty(SupportsGet = true)]
        public string RedirectUrl { get; set; } = "/Index";

        [BindProperty(SupportsGet = true)]
        public int Seconds { get; set; } = 3;

        public void OnGet()
        {
            // 可以做额外安全检查，比如防止空 URL
            if (string.IsNullOrWhiteSpace(RedirectUrl))
            {
                RedirectUrl = "/Index";
            }
        }
    }
}
