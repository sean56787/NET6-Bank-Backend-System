using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Services;
using DotNetSandbox.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Models.Data;

var builder = WebApplication.CreateBuilder(args); // 初始化ASP.NET Core App

var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtConfig["Key"]);

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlite("Data source=user.db"); }); // 有人需要AppDbContext時自動注入
builder.Services.AddScoped<AuthService>(); // TODO: 改自動注入
builder.Services.AddScoped<AdminService>(); // TODO: 改自動注入
builder.Services.AddScoped<UserService>(); // TODO: 改自動注入
builder.Services.AddScoped<IBalanceService, BalanceService>(); // DI
builder.Services.AddAuthentication(options =>
{
    // 告訴 ASP.NET Core 使用 Bearer Token 驗證用戶JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;   // 解析完 Token 自動存進 HttpContext.User
    options.TokenValidationParameters = new TokenValidationParameters
    { 
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,    // 檢查簽章金鑰
    };
});

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
// 測試用 回傳縮排給客戶端
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });
// 測試用

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DotNetSandbox.Data.SeedData.Initialize(context);
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection(); // 強制將 HTTP 轉為 HTTPS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // 讓[ApiController] 的 Controller 路由生效

app.Run();
