using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Services.Interfaces;
using DotNetSandbox.Models.Data;
namespace DotNetSandbox.Services.MiddleWares
{
    public class ExceptionHandlingMW : IMiddleware
    {
        private readonly IErrorLoggingService _errorLoggingService;

        public ExceptionHandlingMW(IErrorLoggingService errorLoggingService)
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
                ServiceResponse<ErrorLog> result =  await _errorLoggingService.LogErrorAsync(ex);

                if (result.Success)
                {
                    var log = result.Data;
                    await context.Response.WriteAsJsonAsync(log);
                }
                else
                {
                    var fail = ServiceResponse<string>.Error(message: "server unvailable now, pls try again later...", statusCode: 500);
                    await context.Response.WriteAsJsonAsync(fail);
                }
                
            }
        }
    }
}
