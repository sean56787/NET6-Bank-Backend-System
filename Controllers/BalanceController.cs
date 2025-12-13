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

        [Authorize(Roles = "admin, user")]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest req)
        {
            try
            {
                var result = await _balanceService.TransferAsync(req, User.Identity?.Name);

                return StatusCode(result.StatusCode, result.Message);
            } 
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest req)
        {
            try
            {
                var result = await _balanceService.WithdrawAsync(req, User.Identity?.Name);

                return StatusCode(result.StatusCode, result.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest req)
        {
            try
            {
                var result = await _balanceService.DepositAsync(req, User.Identity?.Name);

                return StatusCode(result.StatusCode, result.Message);
            } 
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustBalance([FromBody] AdjustBalanceRequest req)
        {
            try
            {
                var result = await _balanceService.AdjustBalanceAsync(req, User.Identity?.Name);

                return StatusCode(result.StatusCode, result.Message);
            } 
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }

        [Authorize(Roles = "admin")]
        [HttpPost("user-transactions")]
        public async Task<IActionResult> GetTransactions([FromBody] TransactionsRequest req)
        {
            try
            {
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
        [Authorize(Roles = "admin")]
        [HttpGet("solo-user-transactions")]
        public async Task<IActionResult> GetSoloTransactions([FromQuery] SoloTransactionRequest req)
        {
            var result = _balanceService.GetSoloTransactions(req, User.Identity?.Name ?? "system");
        }
        */
    }
}
