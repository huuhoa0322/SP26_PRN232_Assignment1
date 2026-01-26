using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.Admin;

public class ReportModel : PageModel
{
    private readonly ApiService _apiService;

    public ReportStatisticsDto? Statistics { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public ReportModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
            return RedirectToPage("/Auth/Login");

        // Validate date range
        if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
        {
            ErrorMessage = "Ngày b?t ??u không ???c sau ngày k?t thúc";
            return Page();
        }

        Statistics = await _apiService.GetReportStatisticsAsync(StartDate, EndDate);
        return Page();
    }

    public async Task<IActionResult> OnGetExportAsync()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
            return RedirectToPage("/Auth/Login");

        // Validate date range
        if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
        {
            ErrorMessage = "Ngày b?t ??u không ???c sau ngày k?t thúc";
            return RedirectToPage("./Report", new { StartDate, EndDate });
        }

        var fileBytes = await _apiService.DownloadReportExcelAsync(StartDate, EndDate);
        if (fileBytes == null)
            return NotFound();

        var fileName = $"BaoCaoThongKe_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        return File(fileBytes, "text/csv", fileName);
    }
} 
