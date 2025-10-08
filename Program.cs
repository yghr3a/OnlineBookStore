using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Services;
using OnlineBookStore.Respository;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace OnlineBookStore
{
    public class Program
    {
        // ע��: ����� Main �������첽��, �Ա���Ӧ������ʱִ���첽�����ݿ�������
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // ע�� Cookie ��֤����
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";       // δ��¼ʱ�ض���
                    options.AccessDeniedPath = "/Account/Denied";
                    options.Cookie.Name = "BookStore.Auth";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7); // �ɱ��ֵ�¼7��
                });

            // ��� HttpContext ����������, �Ա������������з��ʵ�ǰ�����HttpContext
            builder.Services.AddAuthorization();

            // MySQL ����, �������¼����, �ɽ�������Ϣ�Ž�appsetting.json, �����޸�
            var connectionString = "server=localhost;port=3306;database=online_book_store;user=root;password=Abcd753!;";

            // ��AppDbContextע��ɷ���
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // ע�᷺�Ͳִ�����, ע������ע���ʼ���ŷ�������, �൱��ע����Repository���еľ�������
            builder.Services.AddScoped(typeof(Respository<>), typeof(Respository<>));

            // ע��ͼ���������
            builder.Services.AddScoped<BookService, BookService>();

            var app = builder.Build();

            // ��Ӧ������ʱ������ݿ�
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
