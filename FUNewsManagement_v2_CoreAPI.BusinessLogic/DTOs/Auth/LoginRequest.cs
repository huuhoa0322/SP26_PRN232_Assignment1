namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Auth
{
    /// <summary>
    /// DTO cho login request
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; 
    }
}
