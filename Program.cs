using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Services;
using OnlineBookStore.Respository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using OnlineBookStore.Models.Entities;

namespace OnlineBookStore
{
    public class Program
    {
        // 注意: 这里的 Main 方法是异步的, 以便在应用启动时执行异步的数据库填充操作
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // 注册 Cookie 认证服务
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "BookStore.Auth";
                    options.Cookie.HttpOnly = true;           // JS 不能读取，减少 XSS 风险
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 仅 HTTPS 传输
                    options.Cookie.SameSite = SameSiteMode.Lax; // 或 Strict/None (注意与跨站情况)
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true; // 在有效期内每次验证都会刷新过期时间

                });

            // 添加 HttpContext 访问器服务, 以便在其他服务中访问当前请求的HttpContext
            builder.Services.AddHttpContextAccessor();

            // MySQL 配置, 若开发事件充分, 可将配置信息放进appsetting.json, 便于修改
            var connectionString = "server=localhost;port=3306;database=online_book_store;user=root;password=Abcd753!;";

            // 将AppDbContext注册成服务
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // 注册泛型仓储服务, 注意这里注册的始开放泛型类型, 相当于注册了Repository所有的具体类型
            builder.Services.AddScoped(typeof(Respository<>), typeof(Respository<>));

            // 注册图书服务类型
            builder.Services.AddScoped<BookService, BookService>();
            // 注册账户服务类型
            builder.Services.AddScoped<AccountService, AccountService>();
            // 注册用户上下文服务类型
            builder.Services.AddScoped<UserContext, UserContext>();
            // 注册用户密码哈希服务
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            var app = builder.Build();

            // 在应用启动时填充数据库
            using (var scope = app.Services.CreateScope())
            {
                // 提供填充数据操作所需要的服务
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var passwordHashHandler = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

                // 
                await SeedService.SeedBooksAsync(context);
                await SeedService.SeedUserAsync(context, passwordHashHandler);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
