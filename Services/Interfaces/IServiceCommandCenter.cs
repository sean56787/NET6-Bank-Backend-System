using DotNetSandbox.Services.CustomResponse;
namespace DotNetSandbox.Services.Interfaces
{
    public interface IServiceCommandCenter
    {
        Task<SystemResponse> RequestKeyCheck(string requestKey);
    }
}
