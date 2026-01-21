using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.News;

public class SearchModel : PageModel
{
    private readonly ApiService _apiService;

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty(SupportsGet = true)]
    public short? CategoryId { get; set; }

    public List<NewsArticleDto> Articles { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();

    public SearchModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        Categories = await _apiService.GetActiveCategoriesAsync();

        if (!string.IsNullOrEmpty(Keyword) || CategoryId.HasValue)
        {
            Articles = await _apiService.SearchNewsAsync(Keyword, CategoryId);
        }
        else
        {
            Articles = await _apiService.GetActiveNewsAsync();
        }
    }
}
