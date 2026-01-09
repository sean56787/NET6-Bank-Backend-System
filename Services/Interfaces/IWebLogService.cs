using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Models.Data;
namespace DotNetSandbox.Services.Interfaces
{
    public interface IWebLogService
    {
        Task WebLogWarningsToDbAsync(WebLog weblog);
        Task WebLogWarnings(string message, DateTime timestamp, Models.Enums.SecurityLevelType securityLevel, int statusCode);
    }
}
