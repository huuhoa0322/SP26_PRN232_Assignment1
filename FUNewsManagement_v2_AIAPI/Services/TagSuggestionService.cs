using FUNewsManagement_v2_AIAPI.DTOs;
using FUNewsManagement_v2_AIAPI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FUNewsManagement_v2_AIAPI.Services
{
    public class TagSuggestionService : ITagSuggestionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILearningCacheService _learningCacheService;

        public TagSuggestionService(HttpClient httpClient, IConfiguration configuration, ILearningCacheService learningCacheService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _learningCacheService = learningCacheService;
        }

        public async Task<SuggestTagResponse> SuggestTagsAsync(string content)
        {
            var response = new SuggestTagResponse();

            if (string.IsNullOrWhiteSpace(content))
            {
                return response;
            }

            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "API_KEY_CUA_BAN_O_DAY")
            {
                // Fallback simulation if no valid key is provided
                response.SuggestedTags = new List<string> { "News", "Update", "General" };
                return response;
            }

            // Get currently learned tags to hint the AI
            var cache = _learningCacheService.GetCache();
            var learnedTagsHint = cache.Count > 0 
                ? $"Previously learned common tags: {string.Join(", ", cache.Keys.Take(10))}. " 
                : "";

            // User requested more tags, changing the limit from 3-5 to 5-10
            var systemPrompt = $"Extract 5-10 most relevant distinct tags/keywords from the following article content. {learnedTagsHint}Return ONLY a raw JSON array of strings (e.g., [\"Tag1\", \"Tag2\"]). Do not include Markdown blocks (```json) or any other text.";
            var fullPrompt = $"{systemPrompt}\n\nArticle Content:\n{content}";

            var requestBody = new
            {
                contents = new[]
                {
                    new 
                    { 
                        parts = new[] { new { text = fullPrompt } }
                    }
                }
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                var apiUrl = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={apiKey}";
                var apiResponse = await _httpClient.PostAsync(apiUrl, requestContent);
                
                if (apiResponse.IsSuccessStatusCode)
                {
                    var responseString = await apiResponse.Content.ReadAsStringAsync();
                    
                    try 
                    {
                        using JsonDocument doc = JsonDocument.Parse(responseString);
                        var candidates = doc.RootElement.GetProperty("candidates");
                        
                        if (candidates.GetArrayLength() > 0)
                        {
                            var aiMessage = candidates[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text").GetString();
                            
                            if (!string.IsNullOrWhiteSpace(aiMessage))
                            {
                                // Clean up potential markdown blocks if AI ignored instruction
                                var cleanJson = aiMessage.Replace("```json", "").Replace("```", "").Trim();
                                
                                var tags = JsonSerializer.Deserialize<List<string>>(cleanJson);
                                if (tags != null)
                                {
                                    response.SuggestedTags = tags;
                                    
                                    // Calculate simple confidence scores based on Learning Cache hits
                                    foreach (var tag in tags)
                                    {
                                        int score = 50; // Base score
                                        if (cache.TryGetValue(tag, out int frequency))
                                        {
                                            score = Math.Min(99, score + (frequency * 5)); // Boost score up to 99 based on prior picks
                                        }
                                        response.TagConfidence[tag] = score;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception jsonEx)
                    {
                         response.SuggestedTags = new List<string> { "Lỗi Parse JSON", "AI trả về sai format", jsonEx.Message };
                         return response;
                    }
                }
                else
                {
                    var errStr = await apiResponse.Content.ReadAsStringAsync();
                    response.SuggestedTags = new List<string> { $"API Lỗi: {apiResponse.StatusCode}", errStr.Length > 200 ? errStr.Substring(0, 200) : errStr };
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception calling Gemini: {ex.Message}");
                // Inject the exception detail into the tags so we can see it on the screen
                response.SuggestedTags = new List<string> { "Lỗi Exception", ex.Message };
                return response;
            }

            // Ultimate fallback if API fails completely (e.g., bad status code but no throw)
            if (response.SuggestedTags.Count == 0)
            {
                 response.SuggestedTags = new List<string> { "Lỗi API", "Không lấy được dữ liệu" };
            }

            return response;
        }
    }
}
