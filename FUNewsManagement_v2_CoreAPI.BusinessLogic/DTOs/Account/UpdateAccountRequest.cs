namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account
{
    /// <summary>
    /// DTO cho cập nhật account
    /// </summary>
    public class UpdateAccountRequest
    {
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public string? OldPassword { get; set; } // Required khi đổi password
        public string? NewPassword { get; set; }
        public short? AccountRole { get; set; } 
    }
}
