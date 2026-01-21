using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages;

public class IndexModel : PageModel
{
    private readonly ApiService _apiService;

    public List<NewsArticleDto> Articles { get; set; } = new();

    public IndexModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        Articles = await _apiService.GetActiveNewsAsync();
    }
}
