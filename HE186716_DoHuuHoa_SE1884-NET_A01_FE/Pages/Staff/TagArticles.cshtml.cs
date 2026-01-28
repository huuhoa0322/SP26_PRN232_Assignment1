using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.Staff;

public class TagArticlesModel : PageModel
{
    private readonly ApiService _apiService;
    
    public List<NewsArticleDto> Articles { get; set; } = new();
    public TagDto? Tag { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int TagId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;
    
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public TagArticlesModel(ApiService apiService) => _apiService = apiService;

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");

        Tag = await _apiService.GetTagByIdAsync(TagId);
        
        if (Tag == null)
            return RedirectToPage("/Staff/Tags");

        if (PageIndex < 1) PageIndex = 1;

        var allArticles = await _apiService.GetArticlesByTagAsync(TagId);
        
        TotalCount = allArticles.Count;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        if (PageIndex > TotalPages && TotalPages > 0) PageIndex = TotalPages;
        
        Articles = allArticles
            .Skip((PageIndex - 1) * PageSize)
            .Take(PageSize)
            .ToList();
        
        return Page();
    }
}
