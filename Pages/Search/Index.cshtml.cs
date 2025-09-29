using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;

namespace OnlineBookStore.Pages.Search
{
    public class IndexModel : PageModel
    {
        public List<BookViewModel> Books = new();
        public void OnGet()
        {
        }
    }
}
