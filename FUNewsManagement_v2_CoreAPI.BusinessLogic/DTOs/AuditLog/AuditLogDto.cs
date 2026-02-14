using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog
{
    /// <summary>
    /// DTO cho audit log
    /// </summary>
    public class AuditLogDto
    {
        [Key]
        public int LogId { get; set; }
        public short? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public string? BeforeData { get; set; }
        public string? AfterData { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
