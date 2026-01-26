using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Pages.Admin;

public class AccountsModel : PageModel
{
    private readonly ApiService _apiService;

    public List<AccountDto> Accounts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Role { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public AccountsModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
            return RedirectToPage("/Auth/Login");

        // Get message from TempData if exists
        if (TempData["Message"] != null)
        {
            Message = TempData["Message"]?.ToString();
            IsSuccess = TempData["IsSuccess"] != null && (bool)TempData["IsSuccess"];
        }

        var pagedResult = await _apiService.SearchAccountsPagedAsync(Keyword, Role, PageIndex, PageSize);
        if (pagedResult != null)
        {
            Accounts = pagedResult.Items;
            TotalPages = pagedResult.TotalPages;
        }
        else
        {
            Accounts = new List<AccountDto>();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string handler, short? AccountId, string AccountName, string AccountEmail, string? AccountPassword, int AccountRole)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
            return RedirectToPage("/Auth/Login");

        if (handler == "Create")
        {
            var dto = new CreateAccountDto
            {
                AccountName = AccountName,
                AccountEmail = AccountEmail,
                AccountPassword = AccountPassword ?? "",
                AccountRole = AccountRole
            };
            var result = await _apiService.CreateAccountAsync(dto);
            TempData["Message"] = result.Message;
            TempData["IsSuccess"] = result.Success;
        }
        else if (handler == "Update" && AccountId.HasValue)
        {
            var dto = new UpdateAccountDto
            {
                AccountName = AccountName,
                AccountEmail = AccountEmail,
                // Only send password if it's not empty
                AccountPassword = string.IsNullOrWhiteSpace(AccountPassword) ? null : AccountPassword,
                AccountRole = AccountRole
            };
            var result = await _apiService.UpdateAccountAsync(AccountId.Value, dto);
            TempData["Message"] = result.Message;
            TempData["IsSuccess"] = result.Success; 
        }

        // Redirect to GET to avoid form resubmission
        return RedirectToPage("./Accounts", new { Keyword, Role, PageIndex });
    }

    public async Task<IActionResult> OnPostDeleteAsync(short deleteId)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
            return RedirectToPage("/Auth/Login");

        var result = await _apiService.DeleteAccountAsync(deleteId);
        TempData["Message"] = result.Message;
        TempData["IsSuccess"] = result.Success;

        // Redirect to GET to avoid form resubmission
        return RedirectToPage("./Accounts", new { Keyword, Role, PageIndex });
    }
}
