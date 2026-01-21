using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.News;

public class DetailsModel : PageModel
{
    private readonly ApiService _apiService;

    public NewsArticleDto? Article { get; set; }
    public List<NewsArticleDto> RelatedArticles { get; set; } = new();

    public DetailsModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToPage("/Index");

        Article = await _apiService.GetNewsDetailAsync(id);
        
        if (Article != null)
        {
            RelatedArticles = await _apiService.GetRelatedNewsAsync(id);
        }

        return Page();
    }
}
