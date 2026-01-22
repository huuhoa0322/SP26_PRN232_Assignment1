using System.ComponentModel.DataAnnotations;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;

// ===== ACCOUNT DTOs =====
public class AccountDto
{
    public short AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountEmail { get; set; }
    public int? AccountRole { get; set; }
    public string RoleName { get; set; } = null!;
    public int ArticleCount { get; set; }
}

public class CreateAccountDto
{
    [Required(ErrorMessage = "Account name is required")]
    [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
    public string AccountName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
    public string AccountEmail { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(70, MinimumLength = 3, ErrorMessage = "Password must be between 3 and 70 characters")]
    public string AccountPassword { get; set; } = null!;

    [Required(ErrorMessage = "Role is required")]
    [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer)")]
    public int AccountRole { get; set; }
}

public class UpdateAccountDto
{
    [Required(ErrorMessage = "Account name is required")]
    [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters")]
    public string AccountName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
    public string AccountEmail { get; set; } = null!;

    [StringLength(70, MinimumLength = 3, ErrorMessage = "Password must be between 3 and 70 characters")]
    public string? AccountPassword { get; set; }

    [Required(ErrorMessage = "Role is required")]
    [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer)")]
    public int AccountRole { get; set; }
}

// DTO for changing password with verification
public class ChangePasswordDto
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = "New password is required")]
    [StringLength(70, MinimumLength = 3, ErrorMessage = "Password must be between 3 and 70 characters")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = null!;
}

