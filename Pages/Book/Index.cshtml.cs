using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;

namespace OnlineBookStore.Pages.Book
{
    /// <summary>
    /// 图书首页模型, 展示书籍的详细信息
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BookService _bookService;

        // 书籍视图模型
        public BookViewModel Book { get; set; }

        // 书籍编号，从 URL 上获取
        [BindProperty(SupportsGet = true)] public int BookNumber { get; set; }

        public IndexModel(ILogger<IndexModel> logger, BookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public async Task OnGet()
        {
            try
            {
                Book = await _bookService.GetBookInformationByNumberAsync(BookNumber);
                if (Book == null)
                    throw new Exception("Book Not Found");
            }
            catch
            {
                // 如果获取书籍信息失败, 则返回404页面
                NotFound(404);
            }
        }
    }
}
