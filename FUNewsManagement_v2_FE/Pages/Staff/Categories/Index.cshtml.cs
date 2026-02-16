using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FUNewsManagement_v2_FE.Pages.Staff.Categories
{
    public class IndexModel : PageModel
    {
        private readonly CoreApiService _apiService;

        public IndexModel(CoreApiService apiService)
        {
            _apiService = apiService;
        }

        public List<CategoryDto> Categories { get; set; } = new();
        public List<CategoryDto> AllCategories { get; set; } = new(); // For dropdown

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

            // Load Data using OData
            // Filter
            string filter = "";
            if (!string.IsNullOrEmpty(Keyword))
            {
                // Simple search on Name or Description
                // OData: contains(tolower(CategoryName), 'keyword')
                string k = Keyword.ToLower();
                filter = $"contains(tolower(CategoryName), '{k}') or contains(tolower(CategoryDesciption), '{k}')";
            }

            // Pagination
            int skip = (PageIndex - 1) * PageSize;

            var response = await _apiService.GetCategoriesAsync(filter, "CategoryName asc", PageSize, skip);
            
            if (response != null)
            {
                Categories = response.Value ?? new List<CategoryDto>();
                TotalCount = response.Count ?? Categories.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                if (PageIndex > TotalPages && TotalPages > 0)
                {
                    PageIndex = TotalPages;
                    // Reload if page index adjusted? strict OData might return empty if skip > count. 
                    // Let's keep it simple.
                }
            }

            // Load All Categories for Dropdown (Limit 100 for simplicity)
            var allResponse = await _apiService.GetCategoriesAsync("", "CategoryName asc", 100, 0);
            if (allResponse != null)
            {
                AllCategories = allResponse.Value ?? new List<CategoryDto>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string handler, short? CategoryId, string CategoryName, string CategoryDesciption, short? ParentCategoryId, bool IsActive)
        {
            if (HttpContext.Session.GetString("Role") != "Staff" && HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Auth/Login");

            try 
            {
                if (handler == "Create")
                {
                    var dto = new CreateCategoryRequest 
                    { 
                        CategoryName = CategoryName, 
                        CategoryDesciption = CategoryDesciption, 
                        ParentCategoryId = ParentCategoryId,
                        IsActive = IsActive 
                    };
                    var result = await _apiService.CreateCategoryAsync(dto);
                    if (result != null)
                    {
                        IsSuccess = true;
                        Message = "Tạo danh mục thành công";
                    }
                    else
                    {
                        IsSuccess = false;
                        Message = "Tạo danh mục thất bại. Có thể tên đã tồn tại.";
                    }
                }
                else if (handler == "Update" && CategoryId.HasValue)
                {
                    var dto = new UpdateCategoryRequest 
                    { 
                        CategoryName = CategoryName, 
                        CategoryDesciption = CategoryDesciption, 
                        ParentCategoryId = ParentCategoryId,
                        IsActive = IsActive 
                    };
                    var result = await _apiService.UpdateCategoryAsync(CategoryId.Value, dto);
                    if (result != null)
                    {
                        IsSuccess = true;
                        Message = "Cập nhật danh mục thành công";
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

        public async Task<IActionResult> OnPostDeleteAsync(short deleteId)
        {
             if (HttpContext.Session.GetString("Role") != "Staff" && HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Auth/Login");

             var success = await _apiService.DeleteCategoryAsync(deleteId);
             if (success)
             {
                 IsSuccess = true;
                 Message = "Xóa danh mục thành công";
             }
             else
             {
                 IsSuccess = false;
                 Message = "Xóa thất bại. Danh mục có thể đang chứa bài viết.";
             }
             return RedirectToPage(new { Keyword, PageIndex });
        }
        
        // Helper to toggle status if needed, though usually Update handles it.
        // CategoriesController has separate Endpoint for status toggle.
        // We can add a handler for it.
        public async Task<IActionResult> OnPostToggleStatusAsync(short id)
        {
             // Need to implement ToggleStatus in CoreApiService first?
             // It's not in the reviewed snippets of CoreApiService.
             // But CategoriesController has it.
             // If CoreApiService doesn't have it, we use Update with negated IsActive?
             // Or better, just rely on Edit Modal which has IsActive checkbox.
             // Reference UI has a Toggle Switch in the table? 
             // "<td>... <input type='checkbox' ... onchange='toggleStatus'> ...</td>"
             // Reference UI used JS for toggle. 
             // If I want to support that with SSR, I need a form per row or JS call.
             // User said "bê nguyên lại". Reference UI used JS for toggle in the table. 
             // But Wait, I am effectively replacing Client-Side fetch with Server-Side loop.
             // The Toggle Switch in Reference Table triggers `toggleStatus` JS function.
             // I can keep that JS function if I implement the API call.
             // But `CoreApiService` is C#.
             // I will leave Toggle Status as strictly via Edit Modal (Server Side) OR implement a simple JS fetch for it.
             // Reference `Categories.cshtml` snippet:
             // `<input class="form-check-input" type="checkbox" ${cat.isActive ? 'checked' : ''} onchange="toggleStatus(${cat.categoryId}, this)">`
             // This was in MY current `Categories.cshtml` (JS version).
             // In the Reference Project `Categories.cshtml` (lines 85-88):
             // `<span class="badge ...">Active/Inactive</span>` -> It was a BADGE, not a switch!
             // So Reference Project **DID NOT** have a direct switch in table. It likely relied on Edit Modal.
             // My previous scan of Reference `Categories.cshtml` confirmed it used Edit Modal for updates.
             // So I only need Edit logic.
             return RedirectToPage();
        }
    }
}
