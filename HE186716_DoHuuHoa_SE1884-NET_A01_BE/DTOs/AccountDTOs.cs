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
    [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên tài khoản không được vượt quá 100 ký tự")]
    public string AccountName { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
    [StringLength(70, ErrorMessage = "Email không được vượt quá 70 ký tự")] 
    public string AccountEmail { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(70, MinimumLength = 3, ErrorMessage = "Mật khẩu phải có từ 3 đến 70 ký tự")]
    public string AccountPassword { get; set; } = null!;

    [Required(ErrorMessage = "Vai trò là bắt buộc")]
    [Range(1, 2, ErrorMessage = "Vai trò phải là 1 (Staff) hoặc 2 (Lecturer)")]
    public int AccountRole { get; set; }
}

public class UpdateAccountDto
{
    [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên tài khoản không được vượt quá 100 ký tự")]
    public string AccountName { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
    [StringLength(70, ErrorMessage = "Email không được vượt quá 70 ký tự")]
    public string AccountEmail { get; set; } = null!;

    // Only validate if password is provided (not null or empty)
    public string? AccountPassword { get; set; }

    [Required(ErrorMessage = "Vai trò là bắt buộc")]
    [Range(1, 2, ErrorMessage = "Vai trò phải là 1 (Staff) hoặc 2 (Lecturer)")]
    public int AccountRole { get; set; }
}

// DTO for changing password with verification
public class ChangePasswordDto
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
    public string CurrentPassword { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [StringLength(70, MinimumLength = 3, ErrorMessage = "Mật khẩu phải có từ 3 đến 70 ký tự")]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp")]
    public string ConfirmPassword { get; set; } = null!;
}

