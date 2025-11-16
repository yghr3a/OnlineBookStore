using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;

namespace OnlineBookStore.Pages.Book
{
    public class SearchModel : PageModel
    {
        private BookApplication _bookService;

        // 从 URL 上获取搜索关键字
        [BindProperty(SupportsGet = true)] public string KeyWord { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;

        public List<BookViewModel> Books { get; set; } = new List<BookViewModel>();

        public SearchModel(BookApplication bookService)
        {
            _bookService = bookService;
        }

        public async Task OnGetAsync()
        {
            var result = await _bookService.GetSearchedBooksAsync(KeyWord, PageIndex);
            if (result.IsSuccess == true)
            {
                Books = result.Data!;
            }
            else
            {
                RedirectToPage("/Shared/Notification", new
                {
                    Message = "搜索图书失败, 请联系网站管理员",
                    RedirectUrl = "/Index",
                    Seconds = 5
                });
            }
        }
    }
}
