using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Services;
using OnlineBookStore.Repository;

namespace OnlineBookStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // MySQL 配置, 若开发事件充分, 可将配置信息放进appsetting.json, 便于修改
            var connectionString = "server=localhost;port=3306;database=demo;user=root;password=Abcd753!;";

            // 将AppDbContext注册成服务
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // 注册泛型仓储服务, 注意这里注册的始开放泛型类型, 相当于注册了Repository所有的具体类型
            builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));

            // 注册图书服务类型
            builder.Services.AddScoped<BookService, BookService>();

            var app = builder.Build();

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
