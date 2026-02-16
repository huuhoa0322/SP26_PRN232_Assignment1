using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagement_v2_FE.Pages.Staff.News
{
    public class IndexModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // Check authentication
            if (HttpContext.Session.GetString("JwtToken") == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Check if Staff or Admin
            var role = HttpContext.Session.GetString("Role");
            if (role != "Staff" && role != "Admin")
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
