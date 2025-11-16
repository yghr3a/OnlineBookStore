using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;
using static System.Reflection.Metadata.BlobBuilder;

namespace OnlineBookStore.Pages
{
    [Authorize] // 确保用户已登录
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BookApplication _bookService;

        public List<BookViewModel> Books { get; set; } = new List<BookViewModel>();

        // 当前页码，从 URL 上获取
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        // 每页多少条数据，可以写死，也可以通过 URL 传递
        public int PageSize { get; set; } = 50;

        public IndexModel(ILogger<IndexModel> logger, BookApplication bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        // 业务方法不能使用 async void 类型函数
        // [2025/10/3] 改为使用async Task类型, async void不推荐使用, 除非是事件处理器
        // [2025/10/4] 增加分页参数
        public async Task OnGet()
        {
            Books = await _bookService.GetPopularBooksAsync(PageIndex, PageSize);
        }
    }
}
