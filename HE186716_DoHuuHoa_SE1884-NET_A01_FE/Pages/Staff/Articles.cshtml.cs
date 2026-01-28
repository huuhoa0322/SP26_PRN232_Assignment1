using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.Staff;

public class ArticlesModel : PageModel
{
    private readonly ApiService _apiService;
    public List<NewsArticleDto> Articles { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
    public List<TagDto> Tags { get; set; } = new();
    
    [TempData]
    public string? Message { get; set; }
    
    [TempData]
    public bool IsSuccess { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty(SupportsGet = true)]
    public short? SearchCategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? SearchStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
    
    // Current logged-in user ID for permission checking
    public short CurrentUserId { get; set; }

    public ArticlesModel(ApiService apiService) => _apiService = apiService;

    private short GetUserId() => short.TryParse(HttpContext.Session.GetString("UserId"), out var id) ? id : (short)0;

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");

        var userId = GetUserId();
        
        if (userId == 0)
        {
            TempData["Message"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
            TempData["IsSuccess"] = false;
            return RedirectToPage("/Auth/Login");
        }

        // Validate date range
        if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
        {
            Message = "Ngày bắt đầu không được sau ngày kết thúc";
            IsSuccess = false;
            StartDate = null;
            EndDate = null;
        }

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(string? NewsTitle, string Headline, 
        string? NewsContent, string? NewsSource, short CategoryId, bool NewsStatus, List<int>? TagIds)
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");

        var userId = GetUserId();
        
        if (userId == 0)
        {
            Message = "Lỗi: Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.";
            IsSuccess = false;
            return RedirectToPage("/Auth/Login");
        }

        var dto = new CreateNewsArticleDto
        {
            NewsTitle = NewsTitle, 
            Headline = Headline, 
            NewsContent = NewsContent,
            NewsSource = NewsSource, 
            CategoryId = CategoryId,
            NewsStatus = NewsStatus,
            TagIds = TagIds
        };
        
        var result = await _apiService.CreateNewsAsync(dto, userId);
        Message = result.Message;
        IsSuccess = result.Success;
        
        if (result.Success)
        {
            return RedirectToPage(new { PageIndex = 1 });
        }
        
        return RedirectToPage(new { Keyword, SearchCategoryId, SearchStatus, StartDate, EndDate, PageIndex });
    }

    public async Task<IActionResult> OnPostUpdateAsync(string ArticleId, string? NewsTitle, string Headline, 
        string? NewsContent, string? NewsSource, short CategoryId, bool NewsStatus, List<int>? TagIds)
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");

        var userId = GetUserId();
        
        if (userId == 0)
        {
            Message = "Lỗi: Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.";
            IsSuccess = false;
            return RedirectToPage("/Auth/Login");
        }

        if (string.IsNullOrEmpty(ArticleId))
        {
            Message = "Lỗi: Không tìm thấy ID bài viết.";
            IsSuccess = false;
            return RedirectToPage(new { Keyword, SearchCategoryId, SearchStatus, StartDate, EndDate, PageIndex });
        }

        var dto = new UpdateNewsArticleDto
        {
            NewsTitle = NewsTitle, 
            Headline = Headline, 
            NewsContent = NewsContent,
            NewsSource = NewsSource, 
            CategoryId = CategoryId,
            NewsStatus = NewsStatus,
            TagIds = TagIds
        };
        
        var result = await _apiService.UpdateNewsAsync(ArticleId, dto, userId);
        Message = result.Message;
        IsSuccess = result.Success;
        
        return RedirectToPage(new { Keyword, SearchCategoryId, SearchStatus, StartDate, EndDate, PageIndex });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string deleteId)
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");
        
        var currentUserId = GetUserId();
        
        // Check ownership before delete
        var article = await _apiService.GetNewsDetailAsync(deleteId);
        if (article == null)
        {
            Message = "Không tìm thấy bài viết";
            IsSuccess = false;
            return RedirectToPage(new { Keyword, SearchCategoryId, SearchStatus, StartDate, EndDate, PageIndex });
        }
        
        // Only allow delete if user is the creator
        if (article.CreatedById != currentUserId)
        {
            Message = "Bạn không có quyền xóa bài viết của người khác";
            IsSuccess = false;
            return RedirectToPage(new { Keyword, SearchCategoryId, SearchStatus, StartDate, EndDate, PageIndex });
        }
        
        var result = await _apiService.DeleteNewsAsync(deleteId);
        Message = result.Message;
        IsSuccess = result.Success;
        
        // PRG Pattern
        return RedirectToPage(new { Keyword, SearchCategoryId, SearchStatus, StartDate, EndDate, PageIndex });
    }

    public async Task<IActionResult> OnPostDuplicateAsync(string duplicateId)
    {
        if (HttpContext.Session.GetString("Role") != "1") return RedirectToPage("/Auth/Login");
        var result = await _apiService.DuplicateNewsAsync(duplicateId, GetUserId());
        Message = result.Message;
        IsSuccess = result.Success;
        
        // PRG Pattern
        return RedirectToPage(new { Keyword, SearchCategoryId, SearchStatus, StartDate, EndDate, PageIndex });
    }

    private async Task LoadDataAsync()
    {
        Categories = await _apiService.GetActiveCategoriesAsync();
        Tags = await _apiService.GetAllTagsAsync(); 
        
        var userId = GetUserId();
        CurrentUserId = userId; // Store for UI permission check
        
        if (userId == 0)
        {
            Articles = new List<NewsArticleDto>();
            TotalPages = 0;
            TotalCount = 0;
            return;
        }
        
        // Staff can view ALL articles (not filtered by userId)
        var pagedResult = await _apiService.SearchNewsPagedAsync(
            Keyword, 
            SearchCategoryId, 
            PageIndex, 
            PageSize, 
            SearchStatus, 
            StartDate, 
            EndDate,
            null); // Pass null to see all articles
            
        if (pagedResult != null)
        {
            Articles = pagedResult.Items;
            TotalPages = pagedResult.TotalPages;
            TotalCount = pagedResult.TotalCount;
        }
        else
        {
            Articles = new List<NewsArticleDto>();
            TotalPages = 0;
            TotalCount = 0;
        }
    }
}
