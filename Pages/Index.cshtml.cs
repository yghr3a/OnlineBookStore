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

        private List<BookViewModel> _books;
        public List<BookViewModel> Books
        {
            get
            {
                if(_books == null)
                    _books =  _bookService.GetPopularBooks().Result;

                return _books;
            }
        }

        public IndexModel(ILogger<IndexModel> logger, BookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public async void OnGet()
        {
            _books = await _bookService.GetPopularBooks();
        }
    }
}
