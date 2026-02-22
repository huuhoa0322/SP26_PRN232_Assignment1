using FUNewsManagement_v2_AIAPI.DTOs;

namespace FUNewsManagement_v2_AIAPI.Services.Interfaces
{
    public interface ITagSuggestionService
    {
        Task<SuggestTagResponse> SuggestTagsAsync(string content);
    }

    public interface ILearningCacheService
    {
        void LearnTags(List<string> selectedTags);
        Dictionary<string, int> GetCache();
    }
}
