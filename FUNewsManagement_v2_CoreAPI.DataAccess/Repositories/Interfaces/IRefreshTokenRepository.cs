using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface cho RefreshToken entity
    /// </summary>
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        /// Lấy refresh token theo token string
        /// </summary>
        Task<RefreshToken?> GetByTokenAsync(string token);

        /// <summary>
        /// Xóa tất cả expired tokens của một account
        /// </summary>
        Task DeleteExpiredTokensAsync(short accountId);
    }
}
