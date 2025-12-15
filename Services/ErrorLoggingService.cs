using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Models.Data;
using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Services.Interfaces;

namespace DotNetSandbox.Services
{
    public class ErrorLoggingService : IErrorLoggingService
    {
        private readonly AppDbContext _context; //scoped!
        public ErrorLoggingService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<ErrorLog>> LogErrorAsync(Exception ex)
        {
            try
            {
                var errorLog = new ErrorLog
                {
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow,
                    SecurityLevel = Models.Enums.SecurityLevelType.DEBUG
                };
                await _context.ErrorLogs.AddAsync(errorLog);
                await _context.SaveChangesAsync();

                return ServiceResponse<ErrorLog>.Ok(data: errorLog, message: "infos or exceptions have been loged", statusCode:201);
            }
            catch(Exception e)
            {
                return ServiceResponse<ErrorLog>.Error(message: $"LogError service unavailable: {e.Message}", statusCode: 503);
            }
        }
    }
}
