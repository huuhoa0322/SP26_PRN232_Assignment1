using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.Staff;

public class MyArticlesModel : PageModel
{
    private readonly ApiService _apiService;
    public List<NewsArticleDto> Articles { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;
    
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public MyArticlesModel(ApiService apiService) => _apiService = apiService;

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("Role") != "1") return RedirectToPage("/Auth/Login");
        
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (short.TryParse(userIdStr, out var userId))
        {
            if (PageIndex < 1) PageIndex = 1;
            
            var allArticles = await _apiService.GetNewsByAuthorAsync(userId);
            
            TotalCount = allArticles.Count;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            
            if (PageIndex > TotalPages && TotalPages > 0) PageIndex = TotalPages;
            
            Articles = allArticles
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
        return Page();
    }
}
