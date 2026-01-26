using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAuthService _authService;

    public AccountService(IAccountRepository accountRepository, IAuthService authService)
    {
        _accountRepository = accountRepository;
        _authService = authService;
    }

    public async Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(MapToDto);
    }

    public async Task<AccountDto?> GetByIdAsync(short id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account == null ? null : MapToDto(account);
    }

    public async Task<IEnumerable<AccountDto>> SearchAsync(string? keyword, int? role = null)
    {
        var accounts = await _accountRepository.SearchAsync(keyword, role);
        return accounts.Select(MapToDto);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto dto)
    {
        // Generate new ID
        var allAccounts = await _accountRepository.GetAllAsync();
        var maxId = allAccounts.Any() ? allAccounts.Max(a => a.AccountId) : (short)0;

        var account = new SystemAccount
        {
            AccountId = (short)(maxId + 1),
            AccountName = dto.AccountName,
            AccountEmail = dto.AccountEmail,
            AccountPassword = dto.AccountPassword,
            AccountRole = dto.AccountRole
        };

        await _accountRepository.AddAsync(account);
        return MapToDto(account);
    }

    public async Task<AccountDto?> UpdateAsync(short id, UpdateAccountDto dto)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return null;

        account.AccountName = dto.AccountName;
        account.AccountEmail = dto.AccountEmail;
        if (!string.IsNullOrEmpty(dto.AccountPassword))
            account.AccountPassword = dto.AccountPassword;
        account.AccountRole = dto.AccountRole;

        await _accountRepository.UpdateAsync(account);
        return MapToDto(account);
    }

    public async Task<(bool Success, string Message)> DeleteAsync(short id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return (false, "Không tìm thấy tài khoản");

        // Check if account has created any articles
        if (await _accountRepository.HasCreatedArticlesAsync(id))
            return (false, "Không thể xóa tài khoản đã tạo bài viết");

        await _accountRepository.DeleteAsync(account);
        return (true, "Xóa tài khoản thành công");
    }

    public async Task<bool> EmailExistsAsync(string email, short? excludeId = null)
    {
        var account = await _accountRepository.GetByEmailAsync(email);
        if (account == null)
            return false;
        if (excludeId.HasValue && account.AccountId == excludeId.Value)
            return false;
        return true;
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(short id, ChangePasswordDto dto)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return (false, "Không tìm thấy tài khoản");

        // Verify current password
        if (account.AccountPassword != dto.CurrentPassword)
            return (false, "Mật khẩu hiện tại không đúng");

        // Update password
        account.AccountPassword = dto.NewPassword;
        await _accountRepository.UpdateAsync(account);

        return (true, "Đổi mật khẩu thành công"); 
    }

    public async Task<PagedResultDto<AccountDto>> SearchPagedAsync(string? keyword, int? role, int pageIndex, int pageSize)
    {
        var query = _accountRepository.GetQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            query = query.Where(a => (a.AccountName != null && a.AccountName.ToLower().Contains(keyword)) ||
                                     (a.AccountEmail != null && a.AccountEmail.ToLower().Contains(keyword)));
        }

        if (role.HasValue)
        {
            query = query.Where(a => a.AccountRole == role.Value);
        }

        var totalCount = await query.CountAsync();
        
        var accounts = await query
            .OrderBy(a => a.AccountId)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = accounts.Select(MapToDto).ToList();

        return new PagedResultDto<AccountDto>(dtos, totalCount, pageIndex, pageSize);
    }

    private AccountDto MapToDto(SystemAccount account)
    {
        return new AccountDto
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountEmail = account.AccountEmail,
            AccountRole = account.AccountRole,
            RoleName = _authService.GetRoleName(account.AccountRole),
            ArticleCount = account.NewsArticles?.Count ?? 0
        };
    }
}
