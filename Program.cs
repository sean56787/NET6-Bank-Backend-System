using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Services.Interfaces;
using DotNetSandbox.Services.Utility;
using DotNetSandbox.Services.MiddleWares;

var builder = WebApplication.CreateBuilder(args); // ��l��ASP.NET Core App

var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtConfig["Key"]);

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlite("Data source=user.db"); }); // ���H�ݭnAppDbContext�ɦ۰ʪ`�J
builder.Services.AddScoped<IAuthService, AuthService>();                // DI
builder.Services.AddScoped<IUserService, UserService>();                // DI
builder.Services.AddScoped<IAdminService, AdminService>();              // DI
builder.Services.AddScoped<IBalanceService, BalanceService>();          // DI
builder.Services.AddScoped<IUserWithdrawCheck, UserWithdrawCheck>();    // DI
builder.Services.AddScoped<IUserDepositCheck, UserDepositCheck>();      // DI
builder.Services.AddScoped<IUserTransferCheck, UserTransferCheck>();    // DI
builder.Services.AddAuthentication(options =>
{
    // �i�D ASP.NET Core �ϥ� Bearer Token ���ҥΤ�JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;   // �ѪR�� Token �۰ʦs�i HttpContext.User
    options.TokenValidationParameters = new TokenValidationParameters
    { 
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,    // �ˬdñ�����_
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

builder.Services.AddControllers(); // �ҥ� MVC �[�c

// �^���Y�Ʈ榡���Ȥ��
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DotNetSandbox.Data.SeedData.Initialize(context);
}

app.UseHttpsRedirection(); // �j��N HTTP �ର HTTPS
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); // ��[ApiController] �� Controller ���ѥͮ�
app.Run();
