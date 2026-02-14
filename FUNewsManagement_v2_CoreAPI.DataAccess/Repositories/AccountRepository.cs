using Microsoft.EntityFrameworkCore;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories
{
    /// <summary>
    /// Triển khai repository cho SystemAccount
    /// </summary>
    public class AccountRepository : GenericRepository<SystemAccount>, IAccountRepository
    {
        public AccountRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<SystemAccount?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.AccountEmail == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(a => a.AccountEmail == email);
        }

        public async Task<IEnumerable<SystemAccount>> SearchAsync(string? name, string? email, short? role)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(a => a.AccountName.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(a => a.AccountEmail.Contains(email));
            }

            if (role.HasValue)
            {
                query = query.Where(a => a.AccountRole == role.Value);
            }

            return await query.ToListAsync();
        }
    }
}
