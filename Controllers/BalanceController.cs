using Microsoft.AspNetCore.Mvc;
using DotNetSandbox.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using DotNetSandbox.Models.DTOs.Input;
using System.Security.Claims;

namespace DotNetSandbox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController:ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [Authorize]
        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustBalance([FromBody] AdjustBalanceRequest req)
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
                    Console.WriteLine(userRole);
                    return StatusCode(403, new { error = "you have no permissions to user this api" });
                }

                var result = await _balanceService.AdjustBalanceAsync(req, User.Identity?.Name ?? "system");

                return StatusCode(result.StatusCode, result.Message);
            } catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }

        [Authorize]
        [HttpPost("user-transactions")]
        public async Task<IActionResult> GetTransactions([FromBody] TransactionsRequest req)
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

                var result = await _balanceService.GetTransactions(req, User.Identity?.Name ?? "system");

                if (result.Success)
                {
                    return StatusCode(result.StatusCode, new { msg = result.Data });
                }
                else
                {
                    return StatusCode(result.StatusCode, new { msg = result.Message });
                }

            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /*
        [Authorize]
        [HttpGet("solo-user-transactions")]
        public async Task<IActionResult> GetSoloTransactions([FromQuery] SoloTransactionRequest req)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized();
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "admin")
                return StatusCode(403, new { msg = "you're not admin" });

            var result = _balanceService.GetSoloTransactions(req, User.Identity?.Name ?? "system");
        }
        */
    }
}
