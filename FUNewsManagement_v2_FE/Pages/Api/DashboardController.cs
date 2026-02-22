using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Dashboard;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly CoreApiService _apiService;
        private readonly AnalyticsApiService _analyticsService;

        public DashboardController(CoreApiService apiService, AnalyticsApiService analyticsService)
        {
            _apiService = apiService;
            _analyticsService = analyticsService;
        }

        // ── Core API stats (accounts, audit logs) ─────────────────────────────

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _apiService.GetDashboardStatsAsync();
            if (result == null)
                return StatusCode(500);
            return Ok(result);
        }

        // ── Analytics API proxies ─────────────────────────────────────────────

        /// <summary>
        /// Proxy: GET /api/dashboard/analytics
        /// Forwards OData $filter to Analytics API (sent by FE via AJAX).
        /// Query string example: ?$filter=NewsStatus eq true
        /// </summary>
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics([FromQuery(Name = "$filter")] string? filter)
        {
            var items = await _analyticsService.GetDashboardAsync(filter);
            if (items == null) return StatusCode(502, new { message = "Analytics API không phản hồi" });
            
            bool isOffline = HttpContext.Items.ContainsKey("IsOfflineMode") && (bool)HttpContext.Items["IsOfflineMode"];
            return isOffline ? Ok(new { value = items, isOffline = true }) : Ok(items);
        }

        /// <summary>
        /// Proxy: GET /api/dashboard/trending?top=N
        /// </summary>
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrending([FromQuery] int top = 10)
        {
            var items = await _analyticsService.GetTrendingAsync(top);
            if (items == null) return StatusCode(502, new { message = "Analytics API không phản hồi" });
            
            bool isOffline = HttpContext.Items.ContainsKey("IsOfflineMode") && (bool)HttpContext.Items["IsOfflineMode"];
            return isOffline ? Ok(new { value = items, isOffline = true }) : Ok(items);
        }

        /// <summary>
        /// Proxy: GET /api/dashboard/export?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd
        /// Returns Excel file download.
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var (bytes, fileName) = await _analyticsService.ExportExcelAsync(startDate, endDate);
            if (bytes == null || bytes.Length == 0)
                return StatusCode(502, new { message = "Không thể xuất báo cáo Excel" });

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                string.IsNullOrEmpty(fileName) ? $"report_{DateTime.Now:yyyyMMdd}.xlsx" : fileName);
        }
    }
}
