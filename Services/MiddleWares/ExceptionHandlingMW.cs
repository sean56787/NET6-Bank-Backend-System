using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Services.Interfaces;
using DotNetSandbox.Models.Data;
namespace DotNetSandbox.Services.MiddleWares
{
    public class ExceptionHandlingMW : IMiddleware
    {
        private readonly IServerLogService _errorLoggingService;

        public ExceptionHandlingMW(IServerLogService errorLoggingService)
        {
            _errorLoggingService = errorLoggingService;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {

            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                SystemResponse<string> result =  await _errorLoggingService.ServerLogExpToDbAsync<string>(ex);

                await context.Response.CompleteAsync();
            }
        }
    }
}
