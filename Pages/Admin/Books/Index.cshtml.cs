using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;
using Entity = OnlineBookStore.Models.Entities.Book;

namespace OnlineBookStore.Pages.Admin.Books
{
    public class IndexModel : PageModel
    {
        private readonly Repository<Entity> _repository;
        private readonly AppDbContext _context;
        [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 15;
        public int TotalPageCount {  get; set; }

        public IndexModel(Repository<Entity> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public IList<Entity> Book { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Book = await _repository.GetPagedAsync(PageIndex, PageSize);
            var totalCount = await _context.Books.CountAsync();
            TotalPageCount = (int)Math.Ceiling(totalCount / (double)PageSize);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if(book is not null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { PageIndex, PageSize});
        }
    }
}
