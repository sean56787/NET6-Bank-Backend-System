using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotNetSandbox.Services;
using System.Security.Claims;
using DotNetSandbox.Models.DTOs.Input;

namespace DotNetSandbox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize]
        [HttpPut("update-user")]
        public IActionResult UpdateUser([FromBody] UpdateUserRequest req)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true) // JWT 是否驗證通過
                {
                    return Unauthorized();
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have to permission to use this api" });
                }
                var userDTO = _adminService.UpdateUser(req);

                if (userDTO != null)
                    return Ok(new { msg = "user update success", userDTO });
                else
                    return BadRequest();
            }
            catch (Exception exp)
            {
                return StatusCode(500, exp.Message);
            }
        }

        [Authorize]
        [HttpPost("create-user")]
        public IActionResult CreateUser([FromBody] CreateUserRequest req)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true) // JWT 是否驗證通過
                {
                    return Unauthorized();
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have to permission to use this api" });
                }

                var result = _adminService.CreateUser(req.Username, req.Password, req.Email, req.Role);
                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, result.Message);
                }
                return Ok(new { msg = result.Message, data = result.Data});
            }
            catch (Exception exp)
            {
                return StatusCode(500, exp.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete-user")]
        public IActionResult DeleteUser([FromBody] DeleteUserRequest req)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true) // JWT 是否驗證通過
                {
                    return Unauthorized();
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have no permission to use this api" });
                }

                var result = _adminService.DeleteUser(req.Username);

                if(!result.Success)
                {
                    return StatusCode(result.StatusCode, new { error = result.Message });
                }
                
                return Ok(new { msg = result.Message });

            } catch (Exception exp)
            {
                return StatusCode(500, exp.Message);
            }
        }

        [Authorize]
        [HttpGet("get-user")]
        public IActionResult GetUser([FromQuery] GetUserRequest req)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true) // JWT 是否驗證通過
                {
                    return Unauthorized();
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have no permission to use this api" });
                }

                var usersDTO = _adminService.GetUser(req.Username, req.Email);
                if (usersDTO != null)
                    return Ok(usersDTO);

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("get-all-users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true) // JWT 是否驗證通過
                {
                    return Unauthorized();
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have no permission to use this api" });
                }

                var usersDTO = _adminService.GetAllUsers();
                if (usersDTO != null)
                    return Ok(usersDTO);
                else return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("frozen-user")]
        public IActionResult FrozenUser([FromBody] FrozenUserRequest req)
        {
            try
            {
                if(User?.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized();
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if(userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have no permission to use this api" });
                }

                var result = _adminService.FrozenUser(req.Username);

                if (!result.Success)
                {
                    if (result.StatusCode == 404)
                    {
                        return StatusCode(result.StatusCode, new { error = result.Message });
                    }
                    else if (result.StatusCode == 409)
                    {
                        return StatusCode(result.StatusCode, new { error = result.Message });
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

                return Ok(new {msg = "user frozen success"});

            } catch (Exception exp)
            {
                return StatusCode(500, exp.Message);
            }
        }
    }
}
