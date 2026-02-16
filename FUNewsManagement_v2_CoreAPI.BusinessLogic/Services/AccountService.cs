using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services
{
    /// <summary>
    /// Triển khai Account management service
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepo, IMapper mapper) 
        {
            _accountRepo = accountRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountDto>> GetAllAsync()
        {
            var accounts = await _accountRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }

        public async Task<AccountDto?> GetByIdAsync(short id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            return account == null ? null : _mapper.Map<AccountDto>(account);
        }

        public async Task<AccountDto> CreateAsync(CreateAccountRequest request)
        {
            // Check duplicate email
            if (await _accountRepo.EmailExistsAsync(request.AccountEmail))
            {
                throw new InvalidOperationException("Email đã tồn tại");
            }

            // Hash password với BCrypt
            // var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.AccountPassword);
            var hashedPassword = request.AccountPassword;

            var maxId = await _accountRepo.GetMaxIdAsync();
            var newId = (short)(maxId + 1);

            var account = new SystemAccount
            {
                AccountId = newId,
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountPassword = hashedPassword,
                AccountRole = request.AccountRole
            };

            var created = await _accountRepo.AddAsync(account);
            return _mapper.Map<AccountDto>(created);
        }

        public async Task<AccountDto?> UpdateAsync(short id, UpdateAccountRequest request)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return null;
            }

            // Update name nếu có
            if (!string.IsNullOrEmpty(request.AccountName))
            {
                account.AccountName = request.AccountName;
            }

            // Update email nếu có (check duplicate)
            if (!string.IsNullOrEmpty(request.AccountEmail) && request.AccountEmail != account.AccountEmail)
            {
                if (await EmailExistsAsync(request.AccountEmail, id))
                {
                    throw new InvalidOperationException("Email đã tồn tại");
                }
                account.AccountEmail = request.AccountEmail;
            }

            // Update password nếu có
            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                // Verify old password
                if (string.IsNullOrEmpty(request.OldPassword) || 
                    // !BCrypt.Net.BCrypt.Verify(request.OldPassword, account.AccountPassword))
                    request.OldPassword != account.AccountPassword)
                {
                    throw new InvalidOperationException("Password cũ không đúng");
                }

                // account.AccountPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                account.AccountPassword = request.NewPassword;
            }

            // Update role nếu có
            if (request.AccountRole.HasValue)
            {
                account.AccountRole = request.AccountRole;
            }

            await _accountRepo.UpdateAsync(account);
            return _mapper.Map<AccountDto>(account);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return false;
            }

            // Check foreign key constraints
            if (await _accountRepo.HasCreatedArticlesAsync(id))
            {
                throw new InvalidOperationException("Tài khoản này đã tạo bài viết, không thể xóa.");
            }

            await _accountRepo.DeleteAsync(account);
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email, short? excludeId = null)
        {
            var exists = await _accountRepo.EmailExistsAsync(email);
            if (!exists || !excludeId.HasValue)
            {
                return exists;
            }

            // Check if existing email belongs to different account
            var account = await _accountRepo.GetByEmailAsync(email);
            return account != null && account.AccountId != excludeId.Value;
        }
    }
}
