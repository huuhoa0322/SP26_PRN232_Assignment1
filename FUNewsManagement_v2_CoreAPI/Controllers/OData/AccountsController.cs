using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers.OData
{
    [Authorize(Roles = "Admin")]
    public class AccountsController : ODataController
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [EnableQuery(MaxTop = 100)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var accounts = await _accountService.GetAllAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OData GetAccounts");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
