using DotNetSandbox.Models.Data;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IServerLogService
    {
        Task<SystemResponse<T>> ServerLogExpToDbAsync<T>(Exception ex);
    }
}
