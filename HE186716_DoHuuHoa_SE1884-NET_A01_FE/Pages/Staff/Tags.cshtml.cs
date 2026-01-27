using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.Staff;

public class TagsModel : PageModel
{
    private readonly ApiService _apiService;
    public List<TagDto> Tags { get; set; } = new();
    
    [TempData]
    public string? Message { get; set; }
    
    [TempData]
    public bool IsSuccess { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    public TagsModel(ApiService apiService) => _apiService = apiService;

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");
            
        Tags = string.IsNullOrWhiteSpace(Keyword) 
            ? await _apiService.GetAllTagsAsync()
            : await _apiService.SearchTagsAsync(Keyword);
            
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string handler, int? TagId, string TagName, string? Note)
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");

        if (handler == "Create")
        {
            var dto = new CreateTagDto { TagName = TagName, Note = Note };
            var result = await _apiService.CreateTagAsync(dto);
            Message = result.Success ? "Tạo tag thành công" : result.Message;
            IsSuccess = result.Success;
        }
        else if (handler == "Update" && TagId.HasValue)
        {
            var dto = new UpdateTagDto { TagName = TagName, Note = Note };
            var result = await _apiService.UpdateTagAsync(TagId.Value, dto);
            Message = result.Success ? "Cập nhật tag thành công" : result.Message;
            IsSuccess = result.Success;
        }

        return RedirectToPage(new { Keyword });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int deleteId)
    {
        if (HttpContext.Session.GetString("Role") != "1") 
            return RedirectToPage("/Auth/Login");
            
        var result = await _apiService.DeleteTagAsync(deleteId);
        Message = result.Success ? "Xóa tag thành công" : result.Message;
        IsSuccess = result.Success;
        
        return RedirectToPage(new { Keyword }); 
    }
}
