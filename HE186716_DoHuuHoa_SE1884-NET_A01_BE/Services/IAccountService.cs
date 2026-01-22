using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Models;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetAllAsync();
    Task<AccountDto?> GetByIdAsync(short id);
    Task<IEnumerable<AccountDto>> SearchAsync(string? keyword, int? role = null);
    Task<AccountDto> CreateAsync(CreateAccountDto dto);
    Task<AccountDto?> UpdateAsync(short id, UpdateAccountDto dto);
    Task<(bool Success, string Message)> DeleteAsync(short id);
    Task<bool> EmailExistsAsync(string email, short? excludeId = null);
    Task<(bool Success, string Message)> ChangePasswordAsync(short id, ChangePasswordDto dto);
}
