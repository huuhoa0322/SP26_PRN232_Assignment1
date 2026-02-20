using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Auth;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Dashboard;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;

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

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<ODataResponse<AccountDto>>(options);
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

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return await response.Content.ReadFromJsonAsync<ODataResponse<AuditLogDto>>(options);
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

        // =========================== CATEGORY Methods ===========================

        public async Task<ODataResponse<CategoryDto>?> GetCategoriesAsync(string? filter = null, string? orderby = null, int? top = null, int? skip = null)
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
                var url = $"/odata/Categories{query}";
                
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<ODataResponse<CategoryDto>>(options);
            }
            catch { return null; }
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(short id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"/api/categories/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryRequest request)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("/api/categories", request);
            if (!response.IsSuccessStatusCode) 
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(short id, UpdateCategoryRequest request)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"/api/categories/{id}", request);
            if (!response.IsSuccessStatusCode) 
            {
                 var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<bool> DeleteCategoryAsync(short id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/categories/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return true;
        }

        public async Task<bool> ToggleCategoryStatusAsync(short id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PatchAsync($"/api/categories/{id}/status", null);
            return response.IsSuccessStatusCode;
        }

        // =========================== TAG Methods ===========================

        public async Task<ODataResponse<TagDto>?> GetTagsAsync(string? filter = null, string? orderby = null, int? top = null, int? skip = null)
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
                var url = $"/odata/Tags{query}";
                
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<ODataResponse<TagDto>>(options);
            }
            catch { return null; }
        }

        public async Task<TagDto?> GetTagByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"/api/tags/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<TagDto>();
        }

        public async Task<TagDto?> CreateTagAsync(CreateTagRequest request)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("/api/tags", request);
            if (!response.IsSuccessStatusCode) 
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<TagDto>();
        }

        public async Task<TagDto?> UpdateTagAsync(int id, UpdateTagRequest request)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"/api/tags/{id}", request);
            if (!response.IsSuccessStatusCode) 
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<TagDto>();
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/tags/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return true;
        }

        // =========================== NEWS ARTICLE Methods ===========================

        public async Task<ODataResponse<NewsArticleDto>?> GetNewsArticlesAsync(string? filter = null, string? orderby = null, int? top = null, int? skip = null)
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

                // NOTE: $expand doesn't work well with DTOs, we rely on Repository Include instead
                // queryParams.Add("$expand=Category,Tags,CreatedBy");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var url = $"/odata/NewsArticles{query}";

                Console.WriteLine($"Calling CoreAPI: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"CoreAPI Error {response.StatusCode}: {errorContent}");
                    return null;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<NewsArticleDto>>(options);

                Console.WriteLine($"Received {result?.Value?.Count ?? 0} news articles from CoreAPI");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetNewsArticlesAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<NewsArticleDto?> GetNewsArticleByIdAsync(string id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"/api/news/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<NewsArticleDto>();
        }

        public async Task<NewsArticleDto?> CreateNewsArticleAsync(CreateNewsArticleRequest request, Stream? imageStream, string? imageName)
        {
            AddAuthorizationHeader();
            
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.NewsTitle), nameof(request.NewsTitle));
            content.Add(new StringContent(request.Headline), nameof(request.Headline));
            if (!string.IsNullOrEmpty(request.NewsContent)) content.Add(new StringContent(request.NewsContent), nameof(request.NewsContent));
            if (!string.IsNullOrEmpty(request.NewsSource)) content.Add(new StringContent(request.NewsSource), nameof(request.NewsSource));
            content.Add(new StringContent(request.CategoryId.ToString()), nameof(request.CategoryId));
            content.Add(new StringContent(request.NewsStatus.ToString()), nameof(request.NewsStatus));
            
            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach(var tagId in request.TagIds)
                {
                    content.Add(new StringContent(tagId.ToString()), "TagIds");
                }
            }

            if (imageStream != null && !string.IsNullOrEmpty(imageName))
            {
                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Adjust based on file type if needed
                content.Add(imageContent, "ImageFile", imageName);
            }

            var response = await _httpClient.PostAsync("/api/news", content);
            
            if (!response.IsSuccessStatusCode) 
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<NewsArticleDto>();
        }

        public async Task<NewsArticleDto?> UpdateNewsArticleAsync(string id, UpdateNewsArticleRequest request, Stream? imageStream, string? imageName)
        {
            AddAuthorizationHeader();
            
            using var content = new MultipartFormDataContent();
            if (request.NewsTitle != null) content.Add(new StringContent(request.NewsTitle), nameof(request.NewsTitle));
            if (request.Headline != null) content.Add(new StringContent(request.Headline), nameof(request.Headline));
            if (request.NewsContent != null) content.Add(new StringContent(request.NewsContent), nameof(request.NewsContent));
            if (request.NewsSource != null) content.Add(new StringContent(request.NewsSource), nameof(request.NewsSource));
            if (request.CategoryId != null) content.Add(new StringContent(request.CategoryId.ToString()!), nameof(request.CategoryId));
            if (request.NewsStatus != null) content.Add(new StringContent(request.NewsStatus.ToString()!), nameof(request.NewsStatus));

            if (request.TagIds != null)
            {
                foreach(var tagId in request.TagIds)
                {
                    content.Add(new StringContent(tagId.ToString()), "TagIds");
                }
            }

            if (imageStream != null && !string.IsNullOrEmpty(imageName))
            {
                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "ImageFile", imageName);
            }

            var response = await _httpClient.PutAsync($"/api/news/{id}", content);
            
             if (!response.IsSuccessStatusCode) 
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<NewsArticleDto>();
        }
        
        public async Task<bool> DeleteNewsArticleAsync(string id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"/api/news/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return true;
        }

        public async Task<NewsArticleDto?> DuplicateNewsArticleAsync(string id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsync($"/api/news/{id}/duplicate", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<NewsArticleDto>();
        }

        public async Task<AccountDto?> GetProfileAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("/api/profile");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<AccountDto>();
        }

        public async Task<AccountDto?> UpdateProfileAsync(UpdateProfileRequest request)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync("/api/profile", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            return await response.Content.ReadFromJsonAsync<AccountDto>();
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

    public class ODataResponse<T>
    {
        [System.Text.Json.Serialization.JsonPropertyName("value")]
        public List<T> Value { get; set; } = new();

        [System.Text.Json.Serialization.JsonPropertyName("@odata.count")]
        public int? Count { get; set; }
    }
}
