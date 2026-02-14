namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account
{
    /// <summary>
    /// DTO cho tạo mới account
    /// </summary>
    public class CreateAccountRequest
    {
        public string AccountName { get; set; } = null!;
        public string AccountEmail { get; set; } = null!;
        public string AccountPassword { get; set; } = null!;
        public short? AccountRole { get; set; } // 1 = Staff, 2 = Lecturer 
    }
}
