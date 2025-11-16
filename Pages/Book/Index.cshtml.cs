using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using OnlineBookStore.Services;
using Microsoft.AspNetCore.Authorization;

namespace OnlineBookStore.Pages.Book
{
    /// <summary>
    /// 图书首页模型, 展示书籍的详细信息
    /// </summary>
    [Authorize] // 确保用户已登录
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BookApplication _bookService;
        private readonly CartApplication _cartApplication;

        // 书籍视图模型
        public BookViewModel Book { get; set; }

        // 书籍编号，从 URL 上获取
        [BindProperty(SupportsGet = true)] public int BookNumber { get; set; }

        public IndexModel(ILogger<IndexModel> logger, BookApplication bookService, CartApplication cartApplication)
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
                // 如果获取书籍信息失败, 则返回404页面
                NotFound(404);
            }
        }

        /// <summary>
        /// 购物车添加书籍操作
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // 为了简单测试, 先不做任何验证

            var result = await _cartApplication.AddBookToUserCart(BookNumber);

            if(result.IsSuccess == true)
            {
                // 登录成功, 跳转到首页
                return RedirectToPage("/Shared/Notification", new
                {
                    Message = "添加到购物车成功",
                    RedirectUrl = $"/Book?BookNumber={BookNumber}", // 重定向当前书籍页面
                    Seconds = 3
                });
            }

            if (result.IsSuccess == false)
            {
                // 登录成功, 跳转到首页
                return RedirectToPage("/Shared/Notification", new
                {
                    // 这里简单就直接把错误信息显示出来了, 实际上可能需要更复杂的处理
                    // 完整的异常处理需要定义业务异常类, 页面层根据业务异常的类型与信息做出对应的处理来引导用户, 这里就不做了
                    Message = $"添加到购物车失败, 失败原因:{result.ErrorMsg}",
                    RedirectUrl = $"/Book?BookNumber={BookNumber}", // 重定向当前书籍页面
                    Seconds = 3
                });
            }

            return Page();
        }
    }
}
