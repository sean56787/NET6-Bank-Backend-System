using DotNetSandbox.Data;
using DotNetSandbox.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // ��l��ASP.NET Core App

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlite("Data source=user.db"); }); // ���H�ݭnAppDbContext�ɦ۰ʪ`�J

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

builder.Services.AddControllers(); // �ҥ� MVC �[�c

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection(); // �j��N HTTP �ର HTTPS

// app.UseAuthorization();

app.MapControllers(); // ��[ApiController] �� Controller ���ѥͮ�

app.Run();
