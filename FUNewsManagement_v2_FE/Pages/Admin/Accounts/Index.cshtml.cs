using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FUNewsManagement_v2_FE.Pages.Admin.Accounts
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

            // Check if Admin
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}

