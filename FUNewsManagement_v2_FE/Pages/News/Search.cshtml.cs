using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_FE.Services;

namespace FUNewsManagement_v2_FE.Pages.News;

public class SearchModel : PageModel
{
    private readonly CoreApiService _coreApi;
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

    public SearchModel(CoreApiService coreApi)
    {
        _coreApi = coreApi;
    }

    public async Task OnGetAsync()
    {
        // Kiểm tra đăng nhập và role
        var role = HttpContext.Session.GetString("Role");
        var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";
        
        // Admin, Staff, Lecturer có thể xem tất cả bài viết (bao gồm cả inactive)
        bool canSeeAll = (role == "Admin" || role == "Staff" || role == "Lecturer" || isAdmin);
        IsAuthenticated = canSeeAll; // For UI toggle like "Inactive" badge

        // Fetch ALL articles and filter in memory for complex filters
        // OData could be used, but in-memory on DTOs avoids OData collection expansion errors
        var newsResponse = await _coreApi.GetNewsArticlesAsync(); // No OData filter, grab all.
        var allArticles = newsResponse?.Value?.ToList() ?? new List<NewsArticleDto>();

        // Dynamically build Dropdowns based on actual available data
        Categories = allArticles
            .Where(a => a.CategoryId.HasValue && !string.IsNullOrEmpty(a.CategoryName))
            .Select(a => new CategoryDto { CategoryId = a.CategoryId.Value, CategoryName = a.CategoryName! })
            .GroupBy(c => c.CategoryId).Select(g => g.First())
            .OrderBy(c => c.CategoryName)
            .ToList();

        Tags = allArticles
            .Where(a => a.Tags != null)
            .SelectMany(a => a.Tags)
            .GroupBy(t => t.TagId).Select(g => g.First())
            .OrderBy(t => t.TagName)
            .ToList();

        // 1. Permission Filter
        if (!canSeeAll)
        {
            allArticles = allArticles.Where(a => a.NewsStatus == true).ToList();
        }

        // 2. Keyword Filter
        if (!string.IsNullOrEmpty(Keyword))
        {
            var keywordLower = Keyword.ToLower();
            allArticles = allArticles.Where(a => 
                (a.NewsTitle != null && a.NewsTitle.ToLower().Contains(keywordLower)) ||
                (a.Headline != null && a.Headline.ToLower().Contains(keywordLower)) ||
                (a.NewsContent != null && a.NewsContent.ToLower().Contains(keywordLower))
            ).ToList();
        }

        // 3. Category Filter
        if (CategoryId.HasValue)
        {
            allArticles = allArticles.Where(a => a.CategoryId == CategoryId.Value).ToList();
        }

        // 4. Tag Filter
        if (TagId.HasValue)
        {
            allArticles = allArticles.Where(a => a.Tags != null && a.Tags.Any(t => t.TagId == TagId.Value)).ToList();
        }

        // 5. Date Filter
        if (StartDate.HasValue)
        {
            allArticles = allArticles.Where(a => a.CreatedDate >= StartDate.Value.Date).ToList();
        }
        if (EndDate.HasValue)
        {
            allArticles = allArticles.Where(a => a.CreatedDate <= EndDate.Value.Date.AddDays(1).AddTicks(-1)).ToList();
        }

        // Apply Sorting
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

    public async Task<IActionResult> OnPostAjaxAsync()
    {
        await OnGetAsync();
        return new JsonResult(new
        {
            articles = Articles,
            totalCount = TotalCount,
            totalPages = TotalPages,
            currentPage = CurrentPage,
            keyword = Keyword,
            categoryId = CategoryId,
            tagId = TagId,
            startDate = StartDate,
            endDate = EndDate,
            isAuthenticated = IsAuthenticated
        });
    }
}
