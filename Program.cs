using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Services;
using OnlineBookStore.Respository;
using Microsoft.AspNetCore.Authentication.Cookies;

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
                    options.LoginPath = "/Account/Login";       // 未登录时重定向
                    options.AccessDeniedPath = "/Account/Denied";
                    options.Cookie.Name = "BookStore.Auth";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7); // 可保持登录7天
                });

            // 添加 HttpContext 访问器服务, 以便在其他服务中访问当前请求的HttpContext
            builder.Services.AddAuthorization();

            // MySQL 配置, 若开发事件充分, 可将配置信息放进appsetting.json, 便于修改
            var connectionString = "server=localhost;port=3306;database=online_book_store;user=root;password=Abcd753!;";

            // 将AppDbContext注册成服务
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // 注册泛型仓储服务, 注意这里注册的始开放泛型类型, 相当于注册了Repository所有的具体类型
            builder.Services.AddScoped(typeof(Respository<>), typeof(Respository<>));

            // 注册图书服务类型
            builder.Services.AddScoped<BookService, BookService>();

            var app = builder.Build();

            // 在应用启动时填充数据库
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await SeedService.SeedBooksAsync(context);
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
