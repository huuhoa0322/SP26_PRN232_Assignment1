using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.News;

public class SearchModel : PageModel
{
    private readonly ApiService _apiService;
    private const int PageSize = 6;

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty(SupportsGet = true)]
    public short? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? TagId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; } = "date"; // "date" or "title"

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public List<NewsArticleDto> Articles { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
    public List<TagDto> Tags { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool IsAuthenticated { get; set; }

    public SearchModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        // Kiểm tra nếu user đã đăng nhập (Admin, Staff, hoặc Lecturer)
        var role = HttpContext.Session.GetString("Role"); 
        IsAuthenticated = !string.IsNullOrEmpty(role);

        Categories = await _apiService.GetActiveCategoriesAsync();
        Tags = await _apiService.GetAllTagsAsync();

        List<NewsArticleDto> allArticles;
        // Get articles with all filters
        if (!string.IsNullOrEmpty(Keyword) || CategoryId.HasValue || TagId.HasValue || StartDate.HasValue || EndDate.HasValue)
        {
            var result = await _apiService.SearchNewsAdvancedAsync(
                keyword: Keyword,
                categoryId: CategoryId,
                tagId: TagId,
                startDate: StartDate,
                endDate: EndDate,
                sortBy: SortBy
            );
            allArticles = result;
        }
        else
        {
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
        }

        // Apply client-side sorting if needed (fallback)
        allArticles = ApplySorting(allArticles, SortBy);
        
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

    private List<NewsArticleDto> ApplySorting(List<NewsArticleDto> articles, string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "title" => articles.OrderBy(a => a.NewsTitle ?? a.Headline).ToList(),
            _ => articles.OrderByDescending(a => a.CreatedDate).ToList()
        };
    }
}
