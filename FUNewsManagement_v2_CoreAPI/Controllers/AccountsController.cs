using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers
{
    /// <summary>
    /// Controller cho Account Management (Admin only)
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")] // ⭐ ADMIN ONLY
    public class AccountsController : ODataController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// GET /odata/Accounts - List accounts với OData queries
        /// Supports: $select, $filter, $orderby, $top, $skip, $count
        /// </summary>
        [HttpGet("odata/Accounts")] 
        [EnableQuery(MaxTop = 100)]
        public async Task<IActionResult> Get()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }
         
        /// <summary>
        /// GET /odata/Accounts({id}) - Get account detail
        /// </summary>
        [HttpGet("odata/Accounts({id})")]
        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] short id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound(new { message = "Account không tồn tại" });
            }

            return Ok(account);
        }

        /// <summary>
        /// POST /api/accounts - Create new account
        /// </summary>
        [HttpPost("api/accounts")]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
        {
            try
            {
                var created = await _accountService.CreateAsync(request);
                return CreatedAtAction(nameof(Get), new { id = created.AccountId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/accounts/{id} - Update account
        /// </summary>
        [HttpPut("api/accounts/{id}")]
        public async Task<IActionResult> Update([FromRoute] short id, [FromBody] UpdateAccountRequest request)
        {
            try
            {
                var updated = await _accountService.UpdateAsync(id, request);
                if (updated == null)
                {
                    return NotFound(new { message = "Account không tồn tại" });
                }

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/accounts/{id} - Delete account
        /// </summary>
        [HttpDelete("api/accounts/{id}")]
        public async Task<IActionResult> Delete([FromRoute] short id)
        {
            try
            {
                var success = await _accountService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Account không tồn tại" });
                }

                return Ok(new { message = "Xóa account thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Foreign key constraint violation
                return BadRequest(new { message = "Không thể xóa account này do có dữ liệu liên quan", detail = ex.Message });
            }
        }
    }
}
