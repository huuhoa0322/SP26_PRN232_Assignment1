using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly ApiService _apiService;

    [BindProperty]
    public string Email { get; set; } = null!;

    [BindProperty]
    public string Password { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public LoginModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public IActionResult OnGet()
    {
        // If already logged in, redirect
        if (HttpContext.Session.GetString("UserName") != null)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Vui lòng nhập email và mật khẩu";
            return Page();
        }

        var result = await _apiService.LoginAsync(Email, Password);
        
        if (result == null)
        {
            ErrorMessage = "Email hoặc mật khẩu không đúng";
            return Page();
        }

        // Save to session
        HttpContext.Session.SetString("UserId", result.AccountId.ToString());
        HttpContext.Session.SetString("UserName", result.AccountName ?? "User");
        HttpContext.Session.SetString("UserEmail", result.AccountEmail ?? "");
        HttpContext.Session.SetString("Role", result.AccountRole?.ToString() ?? "");
        HttpContext.Session.SetString("RoleName", result.RoleName);
        HttpContext.Session.SetString("IsAdmin", result.IsAdmin.ToString());

        // Redirect based on role
        if (result.IsAdmin)
        {
            return RedirectToPage("/Admin/Index");
        }
        else if (result.AccountRole == 1) // Staff
        {
            return RedirectToPage("/Staff/Index");
        }
        else
        {
            return RedirectToPage("/Index");
        }
    }
}
