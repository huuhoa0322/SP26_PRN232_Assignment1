using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface cho AuditLog entity
    /// </summary>
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        /// <summary>
        /// Lấy audit logs theo filters
        /// </summary>
        Task<IEnumerable<AuditLog>> FilterAsync(short? userId, string? entityType, DateTime? from, DateTime? to);
    }
}
