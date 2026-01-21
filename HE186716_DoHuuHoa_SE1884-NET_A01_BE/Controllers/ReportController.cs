using Microsoft.AspNetCore.Mvc;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Get statistics report with optional date range filter
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ReportStatisticsDto>> GetStatistics(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var filter = new ReportFilterDto
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var statistics = await _reportService.GetStatisticsAsync(filter);
        return Ok(statistics);
    }

    /// <summary>
    /// Export statistics report to CSV file
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportToCsv(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var filter = new ReportFilterDto
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var csvBytes = await _reportService.ExportToCsvAsync(filter);
        var fileName = $"NewsReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        return File(csvBytes, "text/csv", fileName);
    }
}

