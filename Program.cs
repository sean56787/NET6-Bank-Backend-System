using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Services;
using DotNetSandbox.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Models.Data;

var builder = WebApplication.CreateBuilder(args); // ��l��ASP.NET Core App

var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtConfig["Key"]);

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlite("Data source=user.db"); }); // ���H�ݭnAppDbContext�ɦ۰ʪ`�J
builder.Services.AddScoped<AuthService>(); // TODO: ��۰ʪ`�J
builder.Services.AddScoped<AdminService>(); // TODO: ��۰ʪ`�J
builder.Services.AddScoped<UserService>(); // TODO: ��۰ʪ`�J
builder.Services.AddScoped<IBalanceService, BalanceService>(); // DI
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

// Add services to the container.

builder.Services.AddControllers(); // �ҥ� MVC �[�c
// ���ե� �^���Y�Ƶ��Ȥ��
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });
// ���ե�

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DotNetSandbox.Data.SeedData.Initialize(context);
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection(); // �j��N HTTP �ର HTTPS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // ��[ApiController] �� Controller ���ѥͮ�

app.Run();
