using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Auth;

namespace FUNewsManagement_v2_FE.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly CoreApiService _apiService;

        [BindProperty]
        public string Email { get; set; } = null!;

        [BindProperty]
        public string Password { get; set; } = null!;

        public LoginModel(CoreApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if already logged in
            if (HttpContext.Session.GetString("JwtToken") != null)
            {
                return RedirectToPage("/Admin/Dashboard/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Call login API
            var response = await _apiService.LoginAsync(Email, Password);
            
            if (response == null)
            {
                TempData["ErrorMessage"] = "Email hoặc mật khẩu không đúng!";
                return Page();
            }

            // Parse JWT to get user info
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(response.AccessToken);
            
            var userEmail = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            var userName = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var role = token.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            // Store in session
            HttpContext.Session.SetString("JwtToken", response.AccessToken);
            HttpContext.Session.SetString("RefreshToken", response.RefreshToken);
            HttpContext.Session.SetString("UserEmail", userEmail ?? "");
            HttpContext.Session.SetString("UserName", userName ?? "");
            HttpContext.Session.SetString("Role", role ?? "");
            HttpContext.Session.SetString("IsAdmin", role == "Admin" ? "True" : "False");

            TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {userName}";

            // Redirect based on role
            if (role == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard/Index");
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
