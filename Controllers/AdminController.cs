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
            try
            {
                var result = await _adminService.UpdateUser(req);

                if (!result.Success)
                {
                    return StatusCode(statusCode: result.StatusCode, new { message = $"server error: {result.Message}" });
                }
                    
                return Ok(new { message = result.Message, info = result.Data });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest req)
        {
            try
            {
                var result = await _adminService.CreateUser(req);
                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, result.Message);
                }
                return Ok(new { message = result.Message, data = result.Data});
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest req)
        {
            try
            {
                var result = await _adminService.DeleteUser(req);

                if(!result.Success)
                {
                    return StatusCode(result.StatusCode, new { error = result.Message });
                }
                
                return Ok(new { message = result.Message });
            } 
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser([FromQuery] GetUserRequest req)
        {
            try
            {
                var result = await _adminService.GetUser(req);
                if (!result.Success)
                    return StatusCode(statusCode:result.StatusCode, new { message = result.Message });

                return Ok(new { message = result.Data });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _adminService.GetAllUsers();
                if (!result.Success)
                    return StatusCode(statusCode: result.StatusCode, new { message = result.Message });

                return Ok(new { message = result.Data });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("frozen-user")]
        public async Task<IActionResult> FrozenUser([FromBody] FrozenUserRequest req)
        {
            try
            {
                var result = await _adminService.FrozenUser(req);

                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, new { error = result.Message });
                }

                return Ok(new {message = result.Message});

            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
