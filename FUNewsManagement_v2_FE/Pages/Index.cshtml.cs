using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FUNewsManagement_v2_FE.Pages
{
    public class IndexModel : PageModel
    {
        public bool IsAuthenticated { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public bool IsAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get user info from session
            IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
            UserEmail = HttpContext.Session.GetString("UserEmail");
            UserName = HttpContext.Session.GetString("UserName");
            Role = HttpContext.Session.GetString("Role");
            IsAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            return Page();
        }
    }
}
 