using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineBookStore.Models.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;

namespace OnlineBookStore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<BookViewModel> Books { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            Books = new List<BookViewModel>
            {
                new BookViewModel { Name = "C#�߼����", CoverImageUrl = "/images/csharp.png" },
                new BookViewModel { Name = "ASP.NET Coreʵս", CoverImageUrl = "/images/aspnet.png" },
                new BookViewModel { Name = "���ģʽָ��", CoverImageUrl = "" },
                new BookViewModel { Name = "C#�߼����", CoverImageUrl = "/images/csharp.png" },
                new BookViewModel { Name = "ASP.NET Coreʵս", CoverImageUrl = "/images/aspnet.png" },
                new BookViewModel { Name = "���ģʽָ��", CoverImageUrl = "" },
                new BookViewModel { Name = "C#�߼����", CoverImageUrl = "/images/csharp.png" },
                new BookViewModel { Name = "ASP.NET Coreʵս", CoverImageUrl = "/images/aspnet.png" },
                new BookViewModel { Name = "���ģʽָ��", CoverImageUrl = "" } 
            };
        }
    }
}
