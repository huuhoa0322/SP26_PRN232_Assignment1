using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FUNewsManagement_v2_FE.Services
{
    /// <summary>
    /// Service để communicate với Core API
    /// </summary>
    public class CoreApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public CoreApiService(HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Login và lấy JWT token
        /// </summary>
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            try
            {
                var request = new { email, password };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
                
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get tất cả accounts (OData)
        /// </summary>
        public async Task<ODataResponse<AccountDto>?> GetAccountsAsync(string? filter = null, string? orderby = null, int? top = null, int? skip = null)
        {
            try
            {
                AddAuthorizationHeader();
                
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(filter)) queryParams.Add($"$filter={filter}");
                if (!string.IsNullOrEmpty(orderby)) queryParams.Add($"$orderby={orderby}");
                if (top.HasValue) queryParams.Add($"$top={top}");
                if (skip.HasValue) queryParams.Add($"$skip={skip}");
                queryParams.Add("$count=true");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var url = $"/odata/Accounts{query}";
                
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<ODataResponse<AccountDto>>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create account
        /// </summary>
        public async Task<AccountDto?> CreateAccountAsync(CreateAccountRequest request)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("/api/accounts", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            return await response.Content.ReadFromJsonAsync<AccountDto>();
        }

        /// <summary>
        /// Update account
        /// </summary>
        public async Task<bool> UpdateAccountAsync(int id, UpdateAccountRequest request)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"/api/accounts/{id}", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            return true;
        }

        /// <summary>
        /// Delete account
        /// </summary>
        public async Task<bool> DeleteAccountAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/accounts/{id}");
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            return true;
        }

        /// <summary>
        /// Get audit logs
        /// </summary>
        public async Task<ODataResponse<AuditLogDto>?> GetAuditLogsAsync(string? filter = null, int? top = 20)
        {
            AddAuthorizationHeader();
            
            var queryParams = new List<string> { "$orderby=Timestamp desc", "$count=true" };
            if (!string.IsNullOrEmpty(filter)) queryParams.Add($"$filter={filter}");
            if (top.HasValue) queryParams.Add($"$top={top}");

            var query = "?" + string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"/odata/AuditLogs{query}");
            
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ODataResponse<AuditLogDto>>();
        }

        /// <summary>
        /// Get dashboard stats
        /// </summary>
        public async Task<DashboardStatsDto?> GetDashboardStatsAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("/api/dashboard/stats");
            
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        }

        /// <summary>
        /// Logout (Revoke refresh token)
        /// </summary>
        public async Task<bool> LogoutAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return false;

            AddAuthorizationHeader();
            var request = new { refreshToken };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/logout", request);
            
            return response.IsSuccessStatusCode;
        }

        private void AddAuthorizationHeader()
        {
            var token = _contextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    #region DTOs

    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }

    public class AccountDto
    {
        public short AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
    }

    public class CreateAccountRequest
    {
        public string AccountName { get; set; } = null!;
        public string AccountEmail { get; set; } = null!;
        public string AccountPassword { get; set; } = null!;
        public short? AccountRole { get; set; }
    }

    public class UpdateAccountRequest
    {
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public string? NewPassword { get; set; }
        public string? OldPassword { get; set; }
        public short? AccountRole { get; set; }
    }

    public class AuditLogDto
    {
        public int LogId { get; set; }
        public short? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public string? BeforeData { get; set; }
        public string? AfterData { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalAccounts { get; set; }
        public int StaffCount { get; set; }
        public int LecturerCount { get; set; }
        public int TotalAuditLogs { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }

    public class RecentActivityDto
    {
        public string? UserEmail { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public DateTime? Timestamp { get; set; }
    }

    public class ODataResponse<T>
    {
        [System.Text.Json.Serialization.JsonPropertyName("value")]
        public List<T> Value { get; set; } = new();

        [System.Text.Json.Serialization.JsonPropertyName("@odata.count")]
        public int? Count { get; set; }
    }

    #endregion
}
