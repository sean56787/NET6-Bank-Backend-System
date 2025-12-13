using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IErrorLoggingService
    {
        Task<ServiceResponse<string>> LogErrorAsync(Exception ex);
    }
}
