namespace DotNetSandbox.Services.MiddleWares
{
    public class ExceptionHandlingMW : IMiddleware
    {
        private readonly ErrorLoggingService _errorLoggingService;

        public ExceptionHandlingMW(ErrorLoggingService errorLoggingService)
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
                await _errorLoggingService.LogErrorAsync(ex);

                context.Response.ContentType = "application/json";
                var res = new
                {
                    StatusCode = 666,
                    Message = "喔喔!?",
                };

                await context.Response.WriteAsJsonAsync(res);
            }
        }
    }
}
