namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Dashboard
{
    /// <summary>
    /// DTO cho dashboard statistics
    /// </summary>
    public class DashboardStatsDto
    {
        public int TotalAccounts { get; set; }
        public int StaffCount { get; set; }
        public int LecturerCount { get; set; }
        public int TotalAuditLogs { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }

    /// <summary>
    /// DTO cho recent activity
    /// </summary>
    public class RecentActivityDto
    {
        public string? UserEmail { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public DateTime? Timestamp { get; set; }
    }
}
