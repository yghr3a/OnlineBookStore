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

        // ҵ�񷽷�����ʹ�� async void ���ͺ���
        // [2025/10/3] ��Ϊʹ��async Task����
        public async Task OnGet()
        {
            Books = await _bookService.GetPopularBooks();
        }
    }
}
