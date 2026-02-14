using Microsoft.EntityFrameworkCore;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories
{
    /// <summary>
    /// Triển khai repository cho RefreshToken
    /// </summary>
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet
                .Include(rt => rt.Account)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task DeleteExpiredTokensAsync(short accountId)
        {
            var expiredTokens = await _dbSet
                .Where(rt => rt.AccountId == accountId && rt.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _dbSet.RemoveRange(expiredTokens);
                await SaveChangesAsync();
            }
        }
    }
}
