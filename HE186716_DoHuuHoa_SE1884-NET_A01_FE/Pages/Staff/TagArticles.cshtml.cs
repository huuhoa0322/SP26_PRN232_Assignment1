using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.Staff;

public class TagArticlesModel : PageModel
{
    private readonly ApiService _apiService;
    
    public List<NewsArticleDto> Articles { get; set; } = new();
    public TagDto? Tag { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int TagId { get; set; }

    public TagArticlesModel(ApiService apiService) => _apiService = apiService;

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");

        Tag = await _apiService.GetTagByIdAsync(TagId);
        
        if (Tag == null)
            return RedirectToPage("/Staff/Tags");

        Articles = await _apiService.GetArticlesByTagAsync(TagId);
        
        return Page();
    }
}
