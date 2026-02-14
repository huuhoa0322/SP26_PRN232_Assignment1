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
            // Tìm user theo email
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
            var accessToken = _jwtHelper.GenerateAccessToken(account);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            // Xóa expired tokens cũ
            await _refreshTokenRepo.DeleteExpiredTokensAsync(account.AccountId);

            // Lưu refresh token vào database
            var refreshTokenExpiryDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!);
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                AccountId = account.AccountId,
                ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepo.AddAsync(refreshTokenEntity);

            // Return LoginResponse
            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
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
