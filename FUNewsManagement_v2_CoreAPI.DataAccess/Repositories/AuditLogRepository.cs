using Microsoft.EntityFrameworkCore;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories
{
    /// <summary>
    /// Triển khai repository cho AuditLog
    /// </summary>
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(FunewsManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            // Include User navigation property
            return await _dbSet
                .Include(log => log.User)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> FilterAsync(short? userId, string? entityType, DateTime? from, DateTime? to)
        {
            var query = _dbSet
                .Include(log => log.User)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(log => log.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                query = query.Where(log => log.EntityType == entityType);
            }

            if (from.HasValue)
            {
                query = query.Where(log => log.Timestamp >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(log => log.Timestamp <= to.Value);
            }

            return await query
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
    }
}
