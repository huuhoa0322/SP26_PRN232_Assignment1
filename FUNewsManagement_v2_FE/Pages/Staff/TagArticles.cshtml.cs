using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement_v2_FE.Services;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;
using System.Security.Claims;

namespace FUNewsManagement_v2_FE.Pages.Staff
{
    public class TagArticlesModel : PageModel
    {
        private readonly CoreApiService _coreApi;

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

        public TagArticlesModel(CoreApiService coreApi)
        {
            _coreApi = coreApi;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check authentication
            if (HttpContext.Session.GetString("JwtToken") == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Check if Staff or Admin
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff" && role != "Admin")
            {
                return RedirectToPage("/Index");
            }

            Tag = await _coreApi.GetTagByIdAsync(TagId);

            if (Tag == null)
                return RedirectToPage("/Staff/Tags/Index");

            if (PageIndex < 1) PageIndex = 1;

            var filter = $"Tags/any(t: t/TagId eq {TagId})";
            
            // Get all articles for this tag (no skip/top yet, because we need to sort numerically)
            var response = await _coreApi.GetNewsArticlesAsync(filter: filter);
            
            if (response != null && response.Value != null)
            {
                var allArticles = response.Value;
                
                // Sort by numeric ID
                var sortedArticles = allArticles.OrderBy(a => 
                {
                    if (long.TryParse(a.NewsArticleId, out long numericId))
                        return numericId;
                    return 0;
                }).ToList();

                TotalCount = sortedArticles.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                
                if (PageIndex > TotalPages && TotalPages > 0) 
                {
                    PageIndex = TotalPages;
                }

                var skip = (PageIndex - 1) * PageSize;
                Articles = sortedArticles.Skip(skip).Take(PageSize).ToList();
            }
            else
            {
                TotalCount = 0;
                TotalPages = 0;
            }

            return Page();
        }
    }
}
