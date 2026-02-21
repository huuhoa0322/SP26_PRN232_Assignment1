using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;
using FUNewsManagement_v2_FE.Services;

namespace FUNewsManagement_v2_FE.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly CoreApiService _coreApi;
        private readonly IConfiguration _config;

        public DetailsModel(CoreApiService coreApi, IConfiguration config)
        {
            _coreApi = coreApi;
            _config  = config;
        }

        public NewsArticleDto? Article { get; private set; }

        /// <summary>
        /// Base URL of Core API — used to prefix relative image paths like "uploads/xxx.jpg"
        /// </summary>
        public string CoreApiBaseUrl { get; private set; } = "";

        public async Task<IActionResult> OnGetAsync(string id)
        {
            CoreApiBaseUrl = _config["CoreApiSettings:BaseUrl"]?.TrimEnd('/') ?? "";
            Article = await _coreApi.GetNewsArticleByIdAsync(id);

            if (Article == null)
            {
                // Article doesn't exist or user is unauthorized (e.g. inactive article for guest)
                return RedirectToPage("/Index");
            }

            // Fallback FE check just in case
            var role = HttpContext.Session.GetString("Role");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            bool canSeeAll = (role == "Admin" || role == "Staff" || role == "Lecturer" || isAdmin);

            if (Article.NewsStatus != true && !canSeeAll)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        /// <summary>Returns full URL for an image. Handles relative and absolute paths.</summary>
        public string GetImageUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return "";
            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return imageUrl;
            // Relative path stored by Core API (e.g. "uploads/abc.jpg")
            return $"{CoreApiBaseUrl}/{imageUrl.TrimStart('/')}";
        }
    }
}
