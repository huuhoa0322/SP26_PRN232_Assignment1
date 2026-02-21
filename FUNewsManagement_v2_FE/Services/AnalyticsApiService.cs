using System.Net.Http.Headers;
using System.Text.Json;

namespace FUNewsManagement_v2_FE.Services
{
    /// <summary>
    /// Service to communicate with the Analytics API (separate from Core API).
    /// Forwards the JWT token from session on every request.
    /// </summary>
    public class AnalyticsApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AnalyticsApiService(HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void AddAuth()
        {
            var token = _contextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<T?> GetAsync<T>(string relativeUrl)
        {
            try
            {
                AddAuth();
                var response = await _httpClient.GetAsync(relativeUrl);
                if (!response.IsSuccessStatusCode) return default;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch { return default; }
        }

        // ── Dashboard ─────────────────────────────────────────────────────────

        /// <summary>GET /api/analytics/dashboard with optional OData $filter</summary>
        public async Task<List<DashboardItemDto>?> GetDashboardAsync(string? odataFilter = null)
        {
            var url = "/api/analytics/dashboard";
            if (!string.IsNullOrEmpty(odataFilter))
                url += $"?$filter={Uri.EscapeDataString(odataFilter)}";

            return await GetAsync<List<DashboardItemDto>>(url);
        }

        // ── Trending ──────────────────────────────────────────────────────────

        /// <summary>GET /api/analytics/trending?$top=N</summary>
        public async Task<List<TrendingArticleDto>?> GetTrendingAsync(int top = 10)
        {
            return await GetAsync<List<TrendingArticleDto>>(
                $"/api/analytics/trending?$top={top}&$orderby=TagCount desc,CreatedDate desc");
        }

        // ── Export ────────────────────────────────────────────────────────────

        /// <summary>GET /api/analytics/export — returns raw bytes for .xlsx download</summary>
        public async Task<(byte[]? Bytes, string FileName)> ExportExcelAsync(
            DateTime? startDate, DateTime? endDate)
        {
            try
            {
                AddAuth();
                var url = "/api/analytics/export";
                var qs = new List<string>();
                if (startDate.HasValue) qs.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue) qs.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                if (qs.Any()) url += "?" + string.Join("&", qs);

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return (null, "");

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                    ?? $"analytics_{DateTime.Now:yyyyMMdd}.xlsx";
                return (bytes, fileName);
            }
            catch { return (null, ""); }
        }

        // ── Recommend ─────────────────────────────────────────────────────────

        /// <summary>GET /api/recommend/{id} — returns up to 3 related articles (anonymous)</summary>
        public async Task<List<RecommendArticleDto>?> GetRecommendAsync(string articleId)
        {
            try
            {
                // No auth needed for recommend endpoint
                var response = await _httpClient.GetAsync($"/api/recommend/{articleId}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<RecommendArticleDto>>(json, _jsonOptions);
            }
            catch { return null; }
        }
    }

    // ── Local DTOs (mirror Analytics API response) ─────────────────────────────

    public class DashboardItemDto
    {
        public string NewsArticleId { get; set; } = "";
        public string? NewsTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public short? CreatedById { get; set; }
        public string? AuthorName { get; set; }
    }

    public class TrendingArticleDto
    {
        public string NewsArticleId { get; set; } = "";
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public int TagCount { get; set; }
    }
}

