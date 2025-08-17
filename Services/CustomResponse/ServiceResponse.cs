using System.Text.Json.Serialization;

namespace DotNetSandbox.Services.CustomResponse
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T? Data { get; set; }

        public static ServiceResponse<T> Ok(T? data = default, string message = "success", int statusCode = 200) =>
            new()
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };

        public static ServiceResponse<T> NotFound(string message = "not found", int statusCode = 404) =>
            new()
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };

        public static ServiceResponse<T> Error(T? data = default, string message = "error", int statusCode = 500) =>
            new()
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
    }
}
