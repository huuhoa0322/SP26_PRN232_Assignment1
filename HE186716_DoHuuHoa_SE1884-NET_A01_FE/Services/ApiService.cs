using System.Net.Http.Json;
using System.Text.Json;
using HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient("NewsAPI");
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    // ===== AUTH =====
    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var request = new LoginRequest { Email = email, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
        }
        return null;
    }

    // ===== NEWS ARTICLES =====
    public async Task<List<NewsArticleDto>> GetActiveNewsAsync()
    {
        var response = await _httpClient.GetAsync("api/news");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<NewsArticleDto>>(_jsonOptions) ?? new();
        }
        return new();
    }

    public async Task<NewsArticleDto?> GetNewsDetailAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/news/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<NewsArticleDto>(_jsonOptions);
        }
        return null;
    }

    public async Task<List<NewsArticleDto>> GetRelatedNewsAsync(string id)
    {
        var response = await _httpClient.GetAsync($"api/news/{id}/related");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<NewsArticleDto>>(_jsonOptions) ?? new();
        }
        return new();
    }

    public async Task<List<NewsArticleDto>> SearchNewsAsync(string? keyword, short? categoryId)
    {
        var url = "api/news/search?";
        if (!string.IsNullOrEmpty(keyword))
            url += $"keyword={Uri.EscapeDataString(keyword)}&";
        if (categoryId.HasValue)
            url += $"categoryId={categoryId}&";
        
        var response = await _httpClient.GetAsync(url.TrimEnd('&', '?'));
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<NewsArticleDto>>(_jsonOptions) ?? new();
        }
        return new();
    }

    // ===== CATEGORIES =====
    public async Task<List<CategoryDto>> GetActiveCategoriesAsync()
    {
        var response = await _httpClient.GetAsync("api/category/active");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<CategoryDto>>(_jsonOptions) ?? new();
        }
        return new();
    }
}
