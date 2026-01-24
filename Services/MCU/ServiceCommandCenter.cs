using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Repositories.Interfaces;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Services.Interfaces;

namespace DotNetSandbox.Services.MCU
{
    public class ServiceCommandCenter : IServiceCommandCenter
    {
        private readonly IUnitOfWork _uow;
        public ServiceCommandCenter(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<SystemResponse> RequestKeyCheck(string requestKey)
        {
            if (string.IsNullOrEmpty(requestKey))
            {
                return SystemResponse.Error(message: "缺少請求唯一標識", statusCode: 401);
            }

            var isProcessed = await _uow.TransferLogs.AnyAsync(x => x.RequestKey == requestKey);
            
            if (isProcessed)
            {
                return SystemResponse.Error(message: "重複請求，請稍後再試", statusCode: 429);
            }

            return SystemResponse.Ok();
        }
    }
}
