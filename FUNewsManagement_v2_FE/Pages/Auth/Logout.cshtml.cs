using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagement_v2_FE.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly Services.CoreApiService _apiService;

        public LogoutModel(Services.CoreApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGet()
        {
            // Call API to revoke refresh token
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _apiService.LogoutAsync(refreshToken);
            }

            // Clear session
            HttpContext.Session.Clear();
            
            TempData["InfoMessage"] = "Đã đăng xuất thành công!";
            
            return RedirectToPage("/Index");
        }
    }
}
