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
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest req)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized();
                }

                var result = await _balanceService.TransferAsync(req, User.Identity?.Name);

                return StatusCode(result.StatusCode, result.Message);
            } 
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest req)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized();
                }

                var result = await _balanceService.WithdrawAsync(req, User.Identity?.Name);

                return StatusCode(result.StatusCode, result.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest req)
        {
            try
            {
                if(User?.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized();
                }

                var result = await _balanceService.DepositAsync(req, User.Identity?.Name);

                return StatusCode(result.StatusCode, result.Message);
            } 
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
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
                    return StatusCode(403, new { error = "you have no permissions to use this api" });
                }

                var result = await _balanceService.AdjustBalanceAsync(req, User.Identity?.Name);

                return StatusCode(result.StatusCode, result.Message);
            } 
            catch(Exception e)
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

                var result = await _balanceService.GetTransactions(req, User.Identity?.Name);

                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, new { error = result.Message });
                }
                
                return Ok(new { message = result.Data });
            } 
            catch (Exception e)
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
                return StatusCode(403, new { message = "you're not admin" });

            var result = _balanceService.GetSoloTransactions(req, User.Identity?.Name ?? "system");
        }
        */
    }
}
