using Microsoft.AspNetCore.Mvc;
using DotNetSandbox.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using DotNetSandbox.Models.DTOs.Input;

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
            var result = await _balanceService.AdjustBalanceAsync(req, User.Identity?.Name ?? "system");
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
