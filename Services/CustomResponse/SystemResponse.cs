using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Services.CustomResponse
{
    public class SystemResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public static SystemResponse Ok(string message = "success", int statusCode = 200) =>
            new()
            {
                Success = true,
                Message = message,
                StatusCode = statusCode,
            };

        public static SystemResponse NotFound(string message = "not found", int statusCode = 404) =>
            new()
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
            };

        public static SystemResponse Error(string message = "error", int statusCode = 500) =>
            new()
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
            };
    }

    public class SystemResponse<T> : SystemResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T? Data { get; set; }

        public static SystemResponse<T>? Ok(T? data = default, string message = "success", int statusCode = 200) =>
            new()
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode,
            };

        public static new SystemResponse<T>? NotFound(string message = "not found", int statusCode = 404) =>
            new()
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
            };

        public static SystemResponse<T>? Error(T? data = default, string message = "error", int statusCode = 500) =>
            new()
            {
                Success = false,
                Data = data,
                Message = message,
                StatusCode = statusCode,
            };
    }
}
