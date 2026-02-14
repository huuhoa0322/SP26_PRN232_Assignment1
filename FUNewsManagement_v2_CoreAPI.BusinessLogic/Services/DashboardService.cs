using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Dashboard;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services
{
    /// <summary>
    /// Triển khai Dashboard service
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IAuditLogRepository _auditLogRepo;

        public DashboardService(
            IAccountRepository accountRepo,
            IAuditLogRepository auditLogRepo)
        {
            _accountRepo = accountRepo;
            _auditLogRepo = auditLogRepo;
        }

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            // Get all accounts
            var accounts = await _accountRepo.GetAllAsync();
            var accountsList = accounts.ToList();

            // Get all audit logs
            var auditLogs = await _auditLogRepo.GetAllAsync();
            var auditLogsList = auditLogs.ToList();

            // Calculate statistics
            var stats = new DashboardStatsDto
            {
                TotalAccounts = accountsList.Count,
                StaffCount = accountsList.Count(a => a.AccountRole == 1),
                LecturerCount = accountsList.Count(a => a.AccountRole == 2),
                TotalAuditLogs = auditLogsList.Count,
                LastLoginTime = auditLogsList
                    .Where(log => log.Action == "Login")
                    .OrderByDescending(log => log.Timestamp)
                    .FirstOrDefault()?.Timestamp,
                RecentActivities = auditLogsList
                    .OrderByDescending(log => log.Timestamp)
                    .Take(10)
                    .Select(log => new RecentActivityDto
                    {
                        UserEmail = log.User?.AccountEmail,
                        Action = log.Action,
                        Entity = log.EntityType,
                        Timestamp = log.Timestamp
                    })
                    .ToList()
            };

            return stats;
        }
    }
}
