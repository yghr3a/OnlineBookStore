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

        // ��ǰҳ�룬�� URL �ϻ�ȡ
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        // ÿҳ���������ݣ�����д����Ҳ����ͨ�� URL ����
        public int PageSize { get; set; } = 50;

        public IndexModel(ILogger<IndexModel> logger, BookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        // ҵ�񷽷�����ʹ�� async void ���ͺ���
        // [2025/10/3] ��Ϊʹ��async Task����, async void���Ƽ�ʹ��, �������¼�������
        // [2025/10/4] ���ӷ�ҳ����
        public async Task OnGet()
        {
            Books = await _bookService.GetPopularBooksAsync(PageIndex, PageSize);
        }
    }
}
