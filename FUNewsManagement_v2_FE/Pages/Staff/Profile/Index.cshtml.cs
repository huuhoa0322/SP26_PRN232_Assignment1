using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Polly.CircuitBreaker;
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

        // Returns true when the exception means the backend is unreachable
        private static bool IsOfflineException(Exception ex) =>
            ex is HttpRequestException ||
            ex is BrokenCircuitException ||
            ex.InnerException is HttpRequestException ||
            ex.InnerException is BrokenCircuitException;

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("JwtToken") == null)
                return RedirectToPage("/Auth/Login");

            try
            {
                var account = await _apiService.GetProfileAsync();
                if (account == null)
                    return RedirectToPage("/Auth/Logout");

                CurrentAccount = account;
                Profile.AccountName = account.AccountName;
                Profile.AccountEmail = account.AccountEmail;
                return Page();
            }
            catch (Exception ex) when (IsOfflineException(ex))
            {
                // Profile has no cached fallback — send user to the Offline page
                return RedirectToPage("/Offline");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var updated = await _apiService.UpdateProfileAsync(Profile);
                if (updated != null)
                {
                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                    HttpContext.Session.SetString("UserName", updated.AccountName ?? "");
                    HttpContext.Session.SetString("UserEmail", updated.AccountEmail ?? "");
                    return RedirectToPage();
                }
            }
            catch (Exception ex) when (IsOfflineException(ex))
            {
                // Write operation failed because API is unreachable
                return RedirectToPage("/Offline");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi cập nhật: " + ex.Message;
            }

            return Page();
        }
    }
}
