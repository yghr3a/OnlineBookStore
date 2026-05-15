using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;
using Entity = OnlineBookStore.Models.Entities.User;

namespace OnlineBookStore.Pages.Admin.UserManage
{
    public class CreateModel : PageModel
    {
        private readonly OnlineBookStore.Repository.AppDbContext _context;

        public CreateModel(OnlineBookStore.Repository.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public new Entity User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
