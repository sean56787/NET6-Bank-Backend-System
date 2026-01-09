using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Services.CustomResponse
{
    public class SystemResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T? Data { get; set; }

        public static SystemResponse<T> Ok(T? data = default, string message = "success", int statusCode = 200) =>
            new()
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode,
            };

        public static SystemResponse<T> NotFound(string message = "not found", int statusCode = 404) =>
            new()
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
            };

        public static SystemResponse<T> Error(T? data = default, string message = "error", int statusCode = 500) =>
            new()
            {
                Success = false,
                Data = data,
                Message = message,
                StatusCode = statusCode,
            };

        /*
        public static IActionResult WebUniResponse(ServiceResponse<T>? result = default)
        {
            if (result != null)
            {
                if (!result.Success)
                    return StatusCode(result.StatusCode, new { error = result.Message });

                return Ok(new { data = result.Data });
            }
        }
        */
    }
}
