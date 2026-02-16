using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<NewsArticleDto>> GetAllAsync();
        Task<NewsArticleDto?> GetByIdAsync(string id);
        Task<NewsArticleDto> CreateAsync(CreateNewsArticleRequest request, short userId);
        Task<NewsArticleDto?> UpdateAsync(string id, UpdateNewsArticleRequest request, short userId);
        Task<bool> DeleteAsync(string id);
        Task<NewsArticleDto?> DuplicateAsync(string id, short userId);
    }
}
