using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Models.Entities;

namespace OnlineBookStore.Respository
{
    /// <summary>
    /// 本应用的数据库上下文, 负责本项目所有的与数据库的操作
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }

        public DbSet<User> Users { get; set; }  
    }
}
