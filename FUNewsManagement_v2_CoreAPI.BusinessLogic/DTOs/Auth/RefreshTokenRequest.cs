namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Auth
{
    /// <summary>
    /// DTO cho refresh token request
    /// </summary>
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = null!; 
    }
}
