using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Dashboard;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces
{
    /// <summary>
    /// Interface cho Dashboard service
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Lấy dashboard statistics
        /// </summary>
        Task<DashboardStatsDto> GetStatsAsync();
    }
}
