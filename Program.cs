using DotNetSandbox.Data;
using DotNetSandbox.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // 初始化ASP.NET Core App

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlite("Data source=user.db"); }); // 有人需要AppDbContext時自動注入

builder.Services.AddScoped<AuthService>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(3000); // http port 3000
    serverOptions.ListenAnyIP(5295, listenOptions => // secure http port 5295
    {
        listenOptions.UseHttps();
    });
});

// Add services to the container.

builder.Services.AddControllers(); // 啟用 MVC 架構

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection(); // 強制將 HTTP 轉為 HTTPS

// app.UseAuthorization();

app.MapControllers(); // 讓[ApiController] 的 Controller 路由生效

app.Run();
