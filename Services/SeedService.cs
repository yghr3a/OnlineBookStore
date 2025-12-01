using Microsoft.AspNetCore.Identity;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Repository;

namespace OnlineBookStore.Services
{
    // 用于初始化和填充数据库的服务, 主要用于测试和开发环境
    public static class SeedService
    {
        /// <summary>
        /// 填充测试书籍数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task SeedBooksAsync(AppDbContext context, NumberFactory numberFactory)
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
                        Number = numberFactory.CreateNumber<Book>(),
                        Name = $"测试书籍{i}",
                        Authors =  $"作者{i}"  ,
                        Publisher = $"出版社{random.Next(1, 10)}",
                        Price = random.Next(20, 90),
                        Sales = random.Next(30, 500),
                        CoverImageUrl = string.Empty, // 可以留空或填默认图片
                        Introduction = $"这是第{i}本书的介绍"
                    });
                }

                await context.Books.AddRangeAsync(books);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 填充测试用户数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="passwordHasher"></param>
        /// <returns></returns>
        public static async Task SeedUserAsync(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            // 检查是否已有数据，避免重复插入
            if (!context.Users.Any())
            {

                var user = new User()
                {
                    Number = 1001,
                    UserName = "root",
                    UserRole = UserRole.Manager,
                    Email = "test@onlinebookstore",
                    IsEmailVerified = true,
                    RegistrationDate = DateTime.Now,
                    Cart = new Cart() // EFCore会自动插入这个Cart对象
                };

                // 生成测试用户的密码哈希, 这里使用简单密码"123456", 同时并没有传递User对象, 因为User对象在哈希计算中未被使用
                // [2025/10/11] 虽然user对象仍然未使用, 但是我删除了PasswordHash属性的required
                var passwordHash = passwordHasher.HashPassword(user, "123456");

                user.PasswordHash = passwordHash;

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
