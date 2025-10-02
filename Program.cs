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

            // MySQL ����, �������¼����, �ɽ�������Ϣ�Ž�appsetting.json, �����޸�
            var connectionString = "server=localhost;port=3306;database=demo;user=root;password=Abcd753!;";

            // ��AppDbContextע��ɷ���
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // ע�᷺�Ͳִ�����, ע������ע���ʼ���ŷ�������, �൱��ע����Repository���еľ�������
            builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));

            // ע��ͼ���������
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
