using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;
using OnlineBookStore.Services;
using Entity = OnlineBookStore.Models.Entities.Book;

namespace OnlineBookStore.Pages.Admin.Books
{
    public class CreateModel : PageModel
    {
        private readonly OnlineBookStore.Repository.AppDbContext _context;
        private NumberFactory _numberFactory;

        public CreateModel(OnlineBookStore.Repository.AppDbContext context,
                           NumberFactory numberFactory)
        {
            _context = context;
            _numberFactory = numberFactory;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Entity Book { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            // 编号还得是要用编号工厂生成
            Book.Number = _numberFactory.CreateNumber<Entity>();
            Book.CoverImageUrl = string.Empty;

            _context.Books.Add(Book);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Shared/Notification", new
            {
                Message = "添加书籍成功！",
                RedirectUrl = "/Admin/Books/Create",
                Seconds = 3
            });
        }
    }
}
