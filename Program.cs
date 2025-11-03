using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Services;
using OnlineBookStore.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using OnlineBookStore.Models.Entities;
using OnlineBookStore.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            // 添加控制器服务, 以支持API控制器可通过DI获取服务实例
            builder.Services.AddControllers();

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

            // 注册 JWT 认证服务
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,          // Token是否包含签发人信息?
                    ValidateAudience = true,        // Token是否包含接收人信息?
                    ValidateLifetime = true,        // Token验证是否有时效性
                    ValidateIssuerSigningKey = true,    
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],      // 从appsettings.json获取签发人信息
                    ValidAudience = builder.Configuration["Jwt:Audience"],  // 从appsettings.json 获取接收人信息
                    IssuerSigningKey = new SymmetricSecurityKey(            //  从appsetting.json 获取原始密钥作为参数生成签发人密钥
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
                };
            });

            // 关闭默认的Claim类型映射, 以防止JWT中的Claim类型被自动转换
            // 甄姬吧坑啊
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


            // 添加 HttpContext 访问器服务, 以便在其他服务中访问当前请求的HttpContext
            builder.Services.AddHttpContextAccessor();

            // MySQL 配置, 若开发时间充分, 可将配置信息放进appsetting.json, 便于修改
            var connectionString = "server=localhost;port=3306;database=online_book_store;User=root;password=Abcd753!;";

            // 将AppDbContext注册成服务
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // 注册泛型仓储服务, 注意这里注册的始开放泛型类型, 相当于注册了Repository所有的具体类型
            builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));

            // 注册图书服务类型
            builder.Services.AddScoped<BookService, BookService>();
            // 注册图书领域服务类型
            builder.Services.AddScoped<BookDomainService, BookDomainService>();

            // 注册购物车应用类型
            builder.Services.AddScoped<CartApplication, CartApplication>();
            // 注册购物车领域服务类型
            builder.Services.AddScoped<CartDomainService, CartDomainService>();
            // 注册购物车工厂服务类型
            builder.Services.AddScoped<CartFactory, CartFactory>();

            // 注册订单应用服务类型
            builder.Services.AddScoped<OrderApplication, OrderApplication>();
            // 注册订单领域服务类型
            builder.Services.AddScoped<OrderDomainService, OrderDomainService>();
            // 注册订单工厂服务类型
            builder.Services.AddScoped<OrderFactory, OrderFactory>();

            // 注册账户应用服务类型
            builder.Services.AddScoped<AccountAppliaction, AccountAppliaction>();
            // 注册用户领域服务类型
            builder.Services.AddScoped<UserDomainService, UserDomainService>();
            // 注册用户上下文服务类型
            builder.Services.AddScoped<UserContext, UserContext>();
            // 注册用户密码哈希服务
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // 注册邮箱相关设置服务, 会从appsetting.json中读取EmailOptions里的设置信息
            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailOptions"));
            // JWT相关设置服务, 会从appsetting.json中读取Jwt里的设置信息
            builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("Jwt"));
            // 注册邮箱发送服务类型
            builder.Services.AddScoped<EmailSendService, EmailSendService>();
            // 注册邮箱验证服务类型
            builder.Services.AddScoped<EmailVerificationTokenService, EmailVerificationTokenService>();

            // 注册工作单元
            builder.Services.AddScoped<UnitOfWork, UnitOfWork>();

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

            app.MapRazorPages();  // Razor页面路由映射
            app.MapControllers(); // API 控制器路由映射

            app.Run();
        }
    }
}
