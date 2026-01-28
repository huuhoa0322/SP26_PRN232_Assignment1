using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages;

public class IndexModel : PageModel
{
    private readonly ApiService _apiService;
    private const int PageSize = 6;

    public List<NewsArticleDto> Articles { get; set; } = new();
    public bool IsAuthenticated { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    public IndexModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        // Kiểm tra nếu user đã đăng nhập (Admin, Staff, hoặc Lecturer)
        var role = HttpContext.Session.GetString("Role");
        IsAuthenticated = !string.IsNullOrEmpty(role);

        List<NewsArticleDto> allArticles;
        if (IsAuthenticated)
        {
            // Admin, Staff, Lecturer có thể xem tất cả bài viết (bao gồm cả inactive)
            allArticles = await _apiService.GetNewsForLecturerAsync();
        }
        else
        {
            // User chưa đăng nhập chỉ xem bài active
            allArticles = await _apiService.GetActiveNewsAsync();
        }

        // Calculate pagination
        TotalCount = allArticles.Count;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        // Ensure CurrentPage is within valid range
        if (CurrentPage < 1) CurrentPage = 1;
        if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

        // Get items for current page
        Articles = allArticles
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList(); 
    }
}
