using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FUNewsManagement_v2_FE.Pages.Staff.Tags
{
    public class IndexModel : PageModel
    {
        private readonly CoreApiService _apiService;

        public IndexModel(CoreApiService apiService)
        {
            _apiService = apiService;
        }

        public List<TagDto> Tags { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        [TempData]
        public string? Message { get; set; }
        [TempData]
        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("JwtToken") == null)
            {
                return RedirectToPage("/Auth/Login");
            }
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff" && role != "Admin")
            {
                return RedirectToPage("/Index");
            }

            if (PageIndex < 1) PageIndex = 1;

            // Filter
            string filter = "";
            if (!string.IsNullOrEmpty(Keyword))
            {
                string k = Keyword.ToLower();
                filter = $"contains(tolower(TagName), '{k}') or contains(tolower(Note), '{k}')";
            }

            // Pagination
            int skip = (PageIndex - 1) * PageSize;

            var response = await _apiService.GetTagsAsync(filter, "TagName asc", PageSize, skip);

            if (response != null)
            {
                Tags = response.Value ?? new List<TagDto>();
                TotalCount = response.Count ?? Tags.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                if (PageIndex > TotalPages && TotalPages > 0)
                {
                    PageIndex = TotalPages;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string handler, int? TagId, string TagName, string? Note)
        {
            if (HttpContext.Session.GetString("Role") != "Staff" && HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Auth/Login");

            try
            {
                if (handler == "Create")
                {
                    var dto = new CreateTagRequest { TagName = TagName, Note = Note };
                    var result = await _apiService.CreateTagAsync(dto);
                    if (result != null)
                    {
                        IsSuccess = true;
                        Message = "Tạo tag thành công";
                    }
                    else
                    {
                        IsSuccess = false;
                        Message = "Tạo tag thất bại. Có thể tên đã tồn tại.";
                    }
                }
                else if (handler == "Update" && TagId.HasValue)
                {
                    var dto = new UpdateTagRequest { TagName = TagName, Note = Note };
                    var result = await _apiService.UpdateTagAsync(TagId.Value, dto);
                   if (result != null)
                    {
                        IsSuccess = true;
                        Message = "Cập nhật tag thành công";
                    }
                    else
                    {
                        IsSuccess = false;
                        Message = "Cập nhật thất bại.";
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = "Lỗi: " + ex.Message;
            }

            return RedirectToPage(new { Keyword, PageIndex });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int deleteId)
        {
            if (HttpContext.Session.GetString("Role") != "Staff" && HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Auth/Login");

            var success = await _apiService.DeleteTagAsync(deleteId);
            if (success)
            {
                IsSuccess = true;
                Message = "Xóa tag thành công";
            }
            else
            {
                IsSuccess = false;
                Message = "Xóa thất bại. Tag có thể đang được sử dụng.";
            }

            return RedirectToPage(new { Keyword, PageIndex });
        }
    }
}
