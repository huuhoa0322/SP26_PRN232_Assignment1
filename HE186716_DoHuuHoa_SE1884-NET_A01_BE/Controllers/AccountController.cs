using Microsoft.AspNetCore.Mvc;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Get all accounts
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll()
    {
        var accounts = await _accountService.GetAllAsync();
        return Ok(accounts);
    }

    /// <summary>
    /// Get account by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetById(short id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account == null)
            return NotFound(new { message = "Không tìm thấy tài khoản" });

        return Ok(account);
    }

    /// <summary>
    /// Search accounts by keyword and/or role with pagination
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PagedResultDto<AccountDto>>> Search(
        [FromQuery] string? keyword, 
        [FromQuery] int? role, 
        [FromQuery] int pageIndex = 1, 
        [FromQuery] int pageSize = 10)
    {
        var result = await _accountService.SearchPagedAsync(keyword, role, pageIndex, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Create a new account
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if email already exists
        if (await _accountService.EmailExistsAsync(dto.AccountEmail))
            return BadRequest(new { message = "Email đã tồn tại" }); 

        var account = await _accountService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = account.AccountId }, account);
    }

    /// <summary>
    /// Update an existing account
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AccountDto>> Update(short id, [FromBody] UpdateAccountDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate password length if provided
        if (!string.IsNullOrEmpty(dto.AccountPassword) && 
            (dto.AccountPassword.Length < 3 || dto.AccountPassword.Length > 70))
            return BadRequest(new { message = "Mật khẩu phải có từ 3 đến 70 ký tự" });  

        // Check if email already exists for another account
        if (await _accountService.EmailExistsAsync(dto.AccountEmail, id))
            return BadRequest(new { message = "Email đã tồn tại" });

        var account = await _accountService.UpdateAsync(id, dto);
        if (account == null)
            return NotFound(new { message = "Không tìm thấy tài khoản" });

        return Ok(account);
    }

    /// <summary>
    /// Delete an account
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(short id)
    {
        var result = await _accountService.DeleteAsync(id);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    /// <summary>
    /// Change account password (requires current password verification)
    /// </summary>
    [HttpPut("{id}/change-password")]
    public async Task<ActionResult> ChangePassword(short id, [FromBody] ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.ChangePasswordAsync(id, dto);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
}

