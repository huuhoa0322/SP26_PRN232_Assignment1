using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Auth
{
    /// <summary>
    /// DTO cho login response
    /// </summary>
    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public AccountDto User { get; set; } = null!; 
    }
}
