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
        private readonly CartService _cartService;

        // �鼮��ͼģ��
        public BookViewModel Book { get; set; }

        // �鼮��ţ��� URL �ϻ�ȡ
        [BindProperty(SupportsGet = true)] public int BookNumber { get; set; }

        public IndexModel(ILogger<IndexModel> logger, BookService bookService, CartService cartService)
        {
            _logger = logger;
            _bookService = bookService;
            _cartService = cartService;
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

        /// <summary>
        /// ���ﳵ����鼮����
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // Ϊ�˼򵥲���, �Ȳ����κ���֤

            var result = await _cartService.AddBookToUserCart(BookNumber);

            return Page();
        }
    }
}
