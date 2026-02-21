using FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.DTOs;

namespace FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.Services.Interfaces
{
    public interface IAnalyticsService
    {
        /// <summary>Returns a flat IQueryable for OData filtering (dashboard).</summary>
        IQueryable<DashboardItemDto> GetDashboardQuery();

        /// <summary>Returns trending articles IQueryable for OData filtering.</summary>
        IQueryable<TrendingArticleDto> GetTrendingQuery();

        /// <summary>Exports an Excel report with optional date filter.</summary>
        Task<byte[]> ExportExcelAsync(DateTime? startDate, DateTime? endDate);
    }
}
