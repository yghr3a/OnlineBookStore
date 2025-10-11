using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;

namespace OnlineBookStore.Pages.Book
{
    /// <summary>
    /// ͼ����ҳģ��, չʾ�鼮����ϸ��Ϣ
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BookService _bookService;

        // �鼮��ͼģ��
        public BookViewModel Book { get; set; }

        // �鼮��ţ��� URL �ϻ�ȡ
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
                // �����ȡ�鼮��Ϣʧ��, �򷵻�404ҳ��
                NotFound(404);
            }
        }
    }
}
