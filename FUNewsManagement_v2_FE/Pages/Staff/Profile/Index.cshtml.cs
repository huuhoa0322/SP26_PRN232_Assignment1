using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;

namespace FUNewsManagement_v2_FE.Pages.Staff.Profile
{
    public class IndexModel : PageModel
    {
        private readonly CoreApiService _apiService;

        public IndexModel(CoreApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public UpdateProfileRequest Profile { get; set; } = new();

        public AccountDto CurrentAccount { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
             // Check authentication
            if (HttpContext.Session.GetString("JwtToken") == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            var account = await _apiService.GetProfileAsync();
            if (account == null)
            {
                return RedirectToPage("/Auth/Logout");
            }

            CurrentAccount = account;
            Profile.AccountName = account.AccountName;
            Profile.AccountEmail = account.AccountEmail;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var updated = await _apiService.UpdateProfileAsync(Profile);
                if (updated != null)
                {
                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                    // Update session name if changed
                    HttpContext.Session.SetString("UserName", updated.AccountName ?? "");
                    HttpContext.Session.SetString("UserEmail", updated.AccountEmail ?? "");
                    
                    return RedirectToPage();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi cập nhật: " + ex.Message;
            }

            return Page();
        }
    }
}
