using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Auth;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Helpers;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services
{
    /// <summary>
    /// Triển khai Authentication service
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo; 
        private readonly JwtHelper _jwtHelper;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(
            IAccountRepository accountRepo,
            IRefreshTokenRepository refreshTokenRepo,
            JwtHelper jwtHelper,
            IMapper mapper,
            IConfiguration configuration)
        {
            _accountRepo = accountRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _jwtHelper = jwtHelper;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // ⭐ Check if this is admin account from appsettings.json (NOT in DB)
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];
            var adminName = _configuration["AdminAccount:Name"];

            if (!string.IsNullOrEmpty(adminEmail) && request.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase))
            {
                // Verify admin password (plain text comparison in config)
                if (request.Password != adminPassword)
                {
                    return null;
                }

                // Create a virtual admin account object for JWT generation
                var adminAccount = new SystemAccount
                {
                    AccountId = 0, // Admin doesn't have ID in DB
                    AccountEmail = adminEmail,
                    AccountName = adminName ?? "Administrator",
                    AccountRole = 0 // 0 = Admin role
                };

                // Generate tokens
                var accessToken = _jwtHelper.GenerateAccessToken(adminAccount);
                var refreshToken = _jwtHelper.GenerateRefreshToken();

                // Note: Admin refresh tokens are not stored in DB
                // They expire after ExpiryMinutes but cannot be refreshed
                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!) * 60,
                    User = new AccountDto
                    {
                        AccountId = 0,
                        AccountEmail = adminEmail,
                        AccountName = adminName ?? "Administrator",
                        AccountRole = 0
                    }
                };
            }

            // Regular account login (from database)
            var account = await _accountRepo.GetByEmailAsync(request.Email);
            if (account == null)
            {
                return null;
            }

            // Verify password với BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.AccountPassword))
            {
                return null;
            }

            // Generate tokens
            var dbAccessToken = _jwtHelper.GenerateAccessToken(account);
            var dbRefreshToken = _jwtHelper.GenerateRefreshToken();

            // Xóa expired tokens cũ
            await _refreshTokenRepo.DeleteExpiredTokensAsync(account.AccountId);

            // Lưu refresh token vào database
            var refreshTokenExpiryDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!);
            var refreshTokenEntity = new RefreshToken
            {
                Token = dbRefreshToken,
                AccountId = account.AccountId,
                ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepo.AddAsync(refreshTokenEntity);

            // Return LoginResponse
            return new LoginResponse
            {
                AccessToken = dbAccessToken,
                RefreshToken = dbRefreshToken,
                ExpiresIn = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!) * 60, // Convert to seconds
                User = _mapper.Map<AccountDto>(account)
            };
        }

        public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
        {
            // Tìm refresh token trong database
            var tokenEntity = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiryDate < DateTime.UtcNow)
            {
                return null;
            }

            // Generate new access token
            var accessToken = _jwtHelper.GenerateAccessToken(tokenEntity.Account);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            // Mark old refresh token as revoked
            tokenEntity.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(tokenEntity);

            // Lưu new refresh token
            var refreshTokenExpiryDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!);
            var newTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                AccountId = tokenEntity.AccountId,
                ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepo.AddAsync(newTokenEntity);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!) * 60,
                User = _mapper.Map<AccountDto>(tokenEntity.Account)
            };
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var tokenEntity = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
            if (tokenEntity == null)
            {
                return false;
            }

            tokenEntity.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(tokenEntity);
            return true;
        }
    }
}
