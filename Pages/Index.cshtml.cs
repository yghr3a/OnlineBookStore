using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;
using static System.Reflection.Metadata.BlobBuilder;

namespace OnlineBookStore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BookService _bookService;

        public List<BookViewModel> Books { get; set; } = new List<BookViewModel>();

        public IndexModel(ILogger<IndexModel> logger, BookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        // 业务方法不能使用 async void 类型函数
        // [2025/10/3] 改为使用async Task类型
        public async Task OnGet()
        {
            Books = await _bookService.GetPopularBooks();
        }
    }
}
