using DotNetSandbox.Data;
using DotNetSandbox.Models.Data;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetSandbox.Services
{
    public class WebLogService : IWebLogService
    {
        private readonly AppDbContext _context;
        private readonly IWebLogService _webLogService;
        public WebLogService(AppDbContext context, IWebLogService webLogService)
        {
            _context = context;
            _webLogService = webLogService;
        }

        public async Task WebLogWarningsToDbAsync(WebLog weblog)
        {
            await _context.AddAsync(weblog);
            await _context.SaveChangesAsync();
        }

        public async Task WebLogWarnings(string message, DateTime timestamp, Models.Enums.SecurityLevelType securityLevel, int statusCode)
        {
            var weblog = new WebLog
            {
                Message = message,
                Timestamp = timestamp,
                SecurityLevel = securityLevel,
                StatusCode = statusCode,
            };
            await _webLogService.WebLogWarningsToDbAsync(weblog);
        }
    }
    
}
            
        
            



