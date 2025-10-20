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
        private readonly CartApplication _cartApplication;

        // �鼮��ͼģ��
        public BookViewModel Book { get; set; }

        // �鼮��ţ��� URL �ϻ�ȡ
        [BindProperty(SupportsGet = true)] public int BookNumber { get; set; }

        public IndexModel(ILogger<IndexModel> logger, BookService bookService, CartApplication cartApplication)
        {
            _logger = logger;
            _bookService = bookService;
            _cartApplication = cartApplication;
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

            var result = await _cartApplication.AddBookToUserCart(BookNumber);

            if(result.IsSuccess == true)
            {
                // ��¼�ɹ�, ��ת����ҳ
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "��ӵ����ﳵ�ɹ�",
                    RedirectUrl = $"/Book?BookNumber={BookNumber}", // �ض���ǰ�鼮ҳ��
                    Seconds = 3
                });
            }

            if (result.IsSuccess == false)
            {
                // ��¼�ɹ�, ��ת����ҳ
                return RedirectToPage("/Shared/Notification", new
                {
                    // ����򵥾�ֱ�ӰѴ�����Ϣ��ʾ������, ʵ���Ͽ�����Ҫ�����ӵĴ���
                    // �������쳣������Ҫ����ҵ���쳣��, ҳ������ҵ���쳣����������Ϣ������Ӧ�Ĵ����������û�, ����Ͳ�����
                    Message = $"��ӵ����ﳵʧ��, ʧ��ԭ��:{result.ErrorMsg}",
                    RedirectUrl = $"/Book?BookNumber={BookNumber}", // �ض���ǰ�鼮ҳ��
                    Seconds = 3
                });
            }

            return Page();
        }
    }
}
