using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllAsync();
    Task<TagDto?> GetByIdAsync(int id);
    Task<IEnumerable<TagDto>> SearchAsync(string? keyword);
    Task<TagDto> CreateAsync(CreateTagDto dto);
    Task<TagDto?> UpdateAsync(int id, UpdateTagDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<IEnumerable<NewsArticleDto>> GetArticlesByTagAsync(int tagId);
}
