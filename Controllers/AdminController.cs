using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Services.Interfaces;

namespace DotNetSandbox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Roles = "admin")]
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest req)
        {
            var result = await _adminService.UpdateUser(req);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { error = result.Message });
                    
            return Ok(new { message = result.Message, data = result.Data });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest req)
        {
            var result = await _adminService.CreateUser(req);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { error = result.Message });

            return Ok(new { message = result.Message, data = result.Data });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest req)
        {
            var result = await _adminService.DeleteUser(req);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { error = result.Message });

            return Ok(new { message = result.Message });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser([FromQuery] GetUserRequest req)
        {
            var result = await _adminService.GetUser(req);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { error = result.Message });

            return Ok(new { data = result.Data });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _adminService.GetAllUsers();
            if (!result.Success)
                return StatusCode(result.StatusCode, new { error = result.Message });

            return Ok(new { data = result.Data });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("frozen-user")]
        public async Task<IActionResult> FrozenUser([FromBody] FrozenUserRequest req)
        {
            var result = await _adminService.FrozenUser(req);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { error = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("test-exp")]
        public async Task<IActionResult> TestExp()
        {
            throw new NullReferenceException();
        }
    }
}
