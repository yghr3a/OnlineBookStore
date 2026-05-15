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
    public class IndexModel : PageModel
    {
        private readonly OnlineBookStore.Repository.AppDbContext _context;

        public IndexModel(OnlineBookStore.Repository.AppDbContext context)
        {
            _context = context;
        }

        public new IList<Entity> User { get;set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _context.Users.ToListAsync();
        }
    }
}
