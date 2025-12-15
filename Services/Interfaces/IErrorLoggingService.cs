using DotNetSandbox.Models.Data;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IErrorLoggingService
    {
        Task<ServiceResponse<ErrorLog>> LogErrorAsync(Exception ex);
    }
}
