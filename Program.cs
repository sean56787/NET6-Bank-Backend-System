using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Services.Interfaces;
using DotNetSandbox.Services.Utility;
using DotNetSandbox.Services;
using DotNetSandbox.Services.MiddleWares;
using DotNetSandbox.Repositories.Interfaces;
using DotNetSandbox.Repositories;

var builder = WebApplication.CreateBuilder(args); // 初始化ASP.NET Core App

var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtConfig["Key"]);

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlite("Data source=user.db"); }); // 有人需要AppDbContext時自動注入
builder.Services.AddScoped<IAuthService, AuthService>();                // SCOPED
builder.Services.AddScoped<IUserService, UserService>();                // SCOPED
builder.Services.AddScoped<IAdminService, AdminService>();              // SCOPED
builder.Services.AddScoped<IBalanceService, BalanceService>();          // SCOPED
builder.Services.AddScoped<IUserWithdrawCheck, UserWithdrawCheck>();    // SCOPED
builder.Services.AddScoped<IUserDepositCheck, UserDepositCheck>();      // SCOPED
builder.Services.AddScoped<IUserTransferCheck, UserTransferCheck>();    // SCOPED
builder.Services.AddScoped<IServerLogService, ServerLogService>();      // SCOPED
builder.Services.AddScoped<IWebLogService, WebLogService>();            // SCOPED
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();                  // SCOPED
builder.Services.AddTransient<ExceptionHandlingMW>();                   // Transient

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
    // serverOptions.ListenAnyIP(5295, listenOptions => // secure http port 5295
    // {
    //     listenOptions.UseHttps();
    // });
});

builder.Services.AddControllers(); // 啟用 MVC 架構

// 回傳縮排格式給客戶端
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   // 允許所有來源
            .AllowAnyMethod()   // 允許 GET, POST, PUT, DELETE 等方法
            .AllowAnyHeader();  // 允許所有 Header
    });

    /* 也可以設定特定來源
        options.AddPolicy("AllowSpecific", policy =>
        {
            policy
                .WithOrigins("https://example.com", "https://another.com")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    */
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMW>(); // 中介層-錯誤捕捉

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DotNetSandbox.Data.SeedData.Initialize(context);
}

app.UseCors("AllowAll");
// app.UseHttpsRedirection(); // 強制將 HTTP 轉為 HTTPS
app.UseAuthentication(); //jwt驗證
app.UseAuthorization(); //jwt授權
app.MapControllers(); // 讓[ApiController] 的 Controller 路由生效 -> 取代UseRouting(導航至API)、UseEndpoints(啟用API)
app.Run();
