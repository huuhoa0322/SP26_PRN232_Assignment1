using FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.DTOs;
using FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_AnalyticsAPI.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    [Authorize]  // Require JWT — Admin only enforced per action
    public class AnalyticsController : ODataController
    {
        private readonly IAnalyticsService _service;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IAnalyticsService service, ILogger<AnalyticsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/analytics/dashboard
        /// Returns a flat list of articles. FE applies OData $filter / $orderby / $select via AJAX.
        /// Example: ?$filter=NewsStatus eq true  or  ?$filter=CategoryId eq 2
        /// Admin only.
        /// </summary>
        [HttpGet("dashboard")]
        [EnableQuery(PageSize = 200)]
        [Authorize(Roles = "Admin")]
        public ActionResult<IQueryable<DashboardItemDto>> GetDashboard()
        {
            _logger.LogInformation("Dashboard requested");
            return Ok(_service.GetDashboardQuery());
        }

        /// <summary>
        /// GET /api/analytics/trending
        /// Returns trending (active, most-tagged, newest) articles.
        /// OData $top / $filter supported.
        /// Admin only.
        /// </summary>
        [HttpGet("trending")]
        [EnableQuery(PageSize = 50)]
        [Authorize(Roles = "Admin")]
        public ActionResult<IQueryable<TrendingArticleDto>> GetTrending()
        {
            _logger.LogInformation("Trending requested");
            return Ok(_service.GetTrendingQuery());
        }

        /// <summary>
        /// GET /api/analytics/export
        /// Downloads an Excel report.
        /// Optional: ?startDate=2026-01-01&amp;endDate=2026-02-28
        /// Admin only.
        /// </summary>
        [HttpGet("export")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Export(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var bytes = await _service.ExportExcelAsync(startDate, endDate);
                var fileName = $"analytics_report_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting analytics report");
                return StatusCode(500, new { message = "Lỗi khi xuất báo cáo Excel" });
            }
        }
    }
}
