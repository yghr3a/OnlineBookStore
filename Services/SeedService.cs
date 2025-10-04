using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;

namespace OnlineBookStore.Services
{
    // 用于初始化和填充数据库的服务, 主要用于测试和开发环境
    public static class SeedService
    {
        public static async Task SeedBooksAsync(AppDbContext context)
        {
            // 检查是否已有数据，避免重复插入
            if (!context.Books.Any())
            {
                var books = new List<Book>();
                var random = new Random();
                for (int i = 1; i <= 4000; i++)
                {
                    books.Add(new Book
                    {
                        Number = 1000 + i,
                        Name = $"测试书籍{i}",
                        Authors = new List<string?>() { $"作者{i}" } ,
                        Publisher = $"出版社{random.Next(1, 10)}",
                        Price = random.Next(20, 90),
                        Sales = random.Next(30, 500),
                        CoverImageUrl = null, // 可以留空或填默认图片
                        Introduction = $"这是第{i}本书的介绍"
                    });
                }

                await context.Books.AddRangeAsync(books);
                await context.SaveChangesAsync();
            }
        }
    }
}
