using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces
{
    /// <summary>
    /// Interface cho Audit Log service
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// Lấy tất cả audit logs (for OData)
        /// </summary>
        Task<IEnumerable<AuditLogDto>> GetAllAsync();

        /// <summary>
        /// Lấy audit log theo ID
        /// </summary>
        Task<AuditLogDto?> GetByIdAsync(int id);

        /// <summary>
        /// Tạo audit log entry
        /// </summary>
        Task CreateAsync(short? userId, string action, string entity, string? beforeData, string? afterData);

        /// <summary>
        /// Filter audit logs
        /// </summary>
        Task<IEnumerable<AuditLogDto>> FilterAsync(short? userId, string? entity, DateTime? fromDate, DateTime? toDate);
    }
}
