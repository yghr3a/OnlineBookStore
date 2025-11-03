using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using OnlineBookStore.Repository;

namespace OnlineBookStore.Services
{
    public class NumberFactory
    {
        private readonly ConcurrentDictionary<string, int> _currentNumbers = new();
        private readonly IServiceProvider _services;

        public NumberFactory(IServiceProvider services)
        {
            _services = services;
        }

        public async Task InitializeAsync()
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 初始化每个实体类型的编号计数
            _currentNumbers["User"] = await db.Users.AnyAsync() ? await db.Users.MaxAsync(u => u.Number) + 1000 : 1000;
            _currentNumbers["Book"] = await db.Books.AnyAsync() ? await db.Books.MaxAsync(b => b.Number) + 1000 : 1000;
            _currentNumbers["Order"] = await db.Orders.AnyAsync() ? await db.Orders.MaxAsync(o => o.Number) + 1000 : 1000;
        }

        public int CreateNumber<T>()
        {
            var key = typeof(T).Name;
            return _currentNumbers.AddOrUpdate(key, 1, (_, current) => current + 1);
        }
    }
}
