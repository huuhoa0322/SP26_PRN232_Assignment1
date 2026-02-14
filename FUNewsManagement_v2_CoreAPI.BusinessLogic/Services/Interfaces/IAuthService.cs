using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Auth;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces
{
    /// <summary>
    /// Interface cho Authentication service
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Đăng nhập với email và password
        /// </summary>
        Task<LoginResponse?> LoginAsync(LoginRequest request); 

        /// <summary>
        /// Refresh access token bằng refresh token
        /// </summary>
        Task<LoginResponse?> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Thu hồi refresh token (logout)
        /// </summary>
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
}
