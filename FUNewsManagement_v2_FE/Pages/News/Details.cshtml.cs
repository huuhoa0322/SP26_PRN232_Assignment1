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

        public async Task OnGetAsync(string id)
        {
            CoreApiBaseUrl = _config["CoreApiSettings:BaseUrl"]?.TrimEnd('/') ?? "";
            Article = await _coreApi.GetNewsArticleByIdAsync(id);
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
