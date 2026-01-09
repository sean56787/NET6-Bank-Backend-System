using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Models.Data;
using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Services.Interfaces;
using System.Text.Json;

namespace DotNetSandbox.Services
{
    public class ServerLogService : IServerLogService
    {
        private readonly AppDbContext _context; //scoped!
        public ServerLogService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<SystemResponse<T>> ServerLogExpToDbAsync<T>(Exception ex)
        {
            try
            {
                var serverLog = new ServerLog
                {
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow,
                    SecurityLevel = Models.Enums.SecurityLevelType.DEBUG
                };
                await _context.ServerLogs.AddAsync(serverLog);
                await _context.SaveChangesAsync();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("server exception occured, logged");
                Console.ResetColor();

                return SystemResponse<T>.Ok(message: "server exceptions have been loged", statusCode: 201);
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LogSystem exception occured: {e.Message}");
                Console.ResetColor();

                return SystemResponse<T>.Error(message: $"LogError service unavailable...{e.Message}", statusCode: 503);
            }
        }
    }
}
