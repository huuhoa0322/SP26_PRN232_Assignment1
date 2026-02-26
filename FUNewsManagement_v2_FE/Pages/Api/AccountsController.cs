using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    /// <summary>
    /// API Proxy to forward requests from Frontend JavaScript to Core API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseApiController
    {
        private readonly CoreApiService _apiService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(CoreApiService apiService, ILogger<AccountsController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts([FromQuery] string? filter, [FromQuery] int? top, [FromQuery] int? skip)
        {
            try
            {
                _logger.LogInformation("GetAccounts called with filter: {Filter}, top: {Top}, skip: {Skip}", filter, top, skip);
                
                var result = await _apiService.GetAccountsAsync(filter, null, top, skip);
                if (result == null)
                {
                    _logger.LogError("GetAccountsAsync returned null");
                    return StatusCode(500, "Failed to fetch accounts from Core API");
                }

                bool isOffline = HttpContext.Items.ContainsKey("IsOfflineMode") && (bool)HttpContext.Items["IsOfflineMode"];

                return Ok(new
                {
                    value = result.Value,
                    count = result.Count,
                    isOffline = isOffline
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAccounts");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] JsonElement request)
        {
            try
            {
                var createRequest = new CreateAccountRequest
                {
                    AccountName = request.GetProperty("accountName").GetString()!,
                    AccountEmail = request.GetProperty("accountEmail").GetString()!,
                    AccountPassword = request.GetProperty("accountPassword").GetString()!,
                    AccountRole = request.TryGetProperty("accountRole", out var roleProp) && roleProp.ValueKind == JsonValueKind.Number 
                        ? (short)roleProp.GetInt32() 
                        : (short)1 // Default to Staff if missing/invalid
                };

                var result = await _apiService.CreateAccountAsync(createRequest);
                if (result == null)
                    return BadRequest("Failed to create account");

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (IsOfflineException(ex)) return OfflineResult();
                _logger.LogError(ex, "Error in CreateAccount");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] JsonElement request)
        {
            try
            {
                var updateRequest = new UpdateAccountRequest
                {
                    AccountName = request.GetProperty("accountName").GetString(),
                    AccountEmail = request.GetProperty("accountEmail").GetString(),
                    NewPassword = request.TryGetProperty("newPassword", out var pwd) ? pwd.GetString() : null,
                    OldPassword = request.TryGetProperty("oldPassword", out var oldPwd) ? oldPwd.GetString() : null,
                    AccountRole = request.TryGetProperty("accountRole", out var roleProp) && roleProp.ValueKind == JsonValueKind.Number 
                        ? (short)roleProp.GetInt32() 
                        : null
                };

                var success = await _apiService.UpdateAccountAsync(id, updateRequest);
                if (!success)
                    return BadRequest("Failed to update account");

                return Ok();
            }
            catch (Exception ex)
            {
                if (IsOfflineException(ex)) return OfflineResult();
                _logger.LogError(ex, "Error in UpdateAccount");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var success = await _apiService.DeleteAccountAsync(id);
                if (!success)
                    return BadRequest("Failed to delete account");

                return Ok();
            }
            catch (Exception ex)
            {
                if (IsOfflineException(ex)) return OfflineResult();
                _logger.LogError(ex, "Error in DeleteAccount");
                return BadRequest(ex.Message);
            }
        }
    }
}
 