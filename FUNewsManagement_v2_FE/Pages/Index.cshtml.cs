using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FUNewsManagement_v2_FE.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Services.CoreApiService _coreApi;

        public bool IsAuthenticated { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public bool IsAdmin { get; set; }

        public List<FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News.NewsArticleDto> Articles { get; set; } = new();
        public int TotalCount { get; set; }

        public IndexModel(Services.CoreApiService coreApi)
        {
            _coreApi = coreApi;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get user info from session
            IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
            UserEmail = HttpContext.Session.GetString("UserEmail");
            UserName = HttpContext.Session.GetString("UserName");
            Role = HttpContext.Session.GetString("Role");
            IsAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            // Filter logic
            string filter = "";
            bool canSeeAll = (Role == "Admin" || Role == "Staff" || Role == "Lecturer" || IsAdmin);
            
            if (!canSeeAll)
            {
                filter = "NewsStatus eq true";
            }

            // Fetch top 6 newest articles
            var response = await _coreApi.GetNewsArticlesAsync(filter: string.IsNullOrEmpty(filter) ? null : filter, orderby: "CreatedDate desc", top: 6);

            if (response != null && response.Value != null)
            {
                Articles = response.Value;
                TotalCount = response.Count ?? 0;
            }

            // Also might want to fetch TotalCount separately if OData $count only counts what's returned?
            // Actually OData $count with top=6 returns the TOTAL count matching the filter, so response.Count is correct.
            TotalCount = response?.Count ?? 0;

            return Page();
        }
    }
}
 