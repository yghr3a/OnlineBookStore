using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;
using Entity = OnlineBookStore.Models.Entities.User;
namespace OnlineBookStore.Pages.Admin.UserManage
{
    public class DetailsModel : PageModel
    {
        private readonly OnlineBookStore.Repository.AppDbContext _context;

        public DetailsModel(OnlineBookStore.Repository.AppDbContext context)
        {
            _context = context;
        }

        public new Entity User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = user;
            }
            return Page();
        }
    }
}
