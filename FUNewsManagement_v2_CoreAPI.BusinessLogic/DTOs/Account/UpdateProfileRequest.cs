using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account
{
    public class UpdateProfileRequest
    {
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string? AccountName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? AccountEmail { get; set; }

        public string? NewPassword { get; set; }
        public string? OldPassword { get; set; }
    }
}
