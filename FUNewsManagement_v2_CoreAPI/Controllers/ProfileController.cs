using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FUNewsManagement_v2_CoreAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public ProfileController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !short.TryParse(idClaim.Value, out short id))
                return Unauthorized();

            var account = await _accountService.GetByIdAsync(id);
            if (account == null) return NotFound();

            // Should we mask password? AccountDto usually doesn't include password anyway.
            return Ok(account);
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !short.TryParse(idClaim.Value, out short id))
                return Unauthorized();

            // Map UpdateProfileRequest to UpdateAccountRequest to reuse service logic
            // Explicitly excluding AccountRole to prevent privilege escalation
            var updateRequest = new UpdateAccountRequest
            {
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                NewPassword = request.NewPassword,
                OldPassword = request.OldPassword,
                AccountRole = null 
            };

            try
            {
                var updated = await _accountService.UpdateAsync(id, updateRequest);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
