using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Services;
using Microsoft.EntityFrameworkCore;

var key = Encoding.ASCII.GetBytes("this_is_a_very_long_secret_key_123456");

var builder = WebApplication.CreateBuilder(args); // ��l��ASP.NET Core App

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlite("Data source=user.db"); }); // ���H�ݭnAppDbContext�ɦ۰ʪ`�J
builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection(); // �j��N HTTP �ର HTTPS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // ��[ApiController] �� Controller ���ѥͮ�

app.Run();
