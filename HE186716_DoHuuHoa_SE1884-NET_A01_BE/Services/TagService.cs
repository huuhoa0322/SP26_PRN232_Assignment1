using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Repositories;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllWithArticlesAsync();
        return tags.Select(MapToDto);
    }

    public async Task<TagDto?> GetByIdAsync(int id)
    {
        var tag = await _tagRepository.GetByIdWithArticlesAsync(id);
        return tag == null ? null : MapToDto(tag);
    }

    public async Task<IEnumerable<TagDto>> SearchAsync(string? keyword)
    {
        var tags = await _tagRepository.SearchAsync(keyword);
        return tags.Select(MapToDto);
    }

    public async Task<TagDto> CreateAsync(CreateTagDto dto)
    {
        // Check if tag name already exists
        if (await _tagRepository.CheckTagNameExistsAsync(dto.TagName))
        {
            throw new InvalidOperationException("Tên tag đã tồn tại");
        }

        var newId = await _tagRepository.GenerateNewIdAsync();

        var tag = new Tag
        {
            TagId = newId,
            TagName = dto.TagName,
            Note = dto.Note
        };

        await _tagRepository.AddAsync(tag);

        return MapToDto(tag);
    }

    public async Task<TagDto?> UpdateAsync(int id, UpdateTagDto dto)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null) return null;

        // Check if tag name already exists (excluding current tag)
        if (await _tagRepository.CheckTagNameExistsAsync(dto.TagName, id))
        {
            throw new InvalidOperationException("Tên tag đã tồn tại");
        }

        tag.TagName = dto.TagName;
        tag.Note = dto.Note;

        await _tagRepository.UpdateAsync(tag);

        return MapToDto(tag);
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null)
        {
            return (false, "Không tìm thấy tag");
        }

        // Check if tag is used in any articles
        if (await _tagRepository.IsUsedInArticlesAsync(id))
        {
            return (false, "Khong the xóa tag vì tag đang được sử dụng trong bài viết");
        }

        await _tagRepository.DeleteAsync(tag);

        return (true, "Xóa tag thành công");
    }

    public async Task<IEnumerable<NewsArticleDto>> GetArticlesByTagAsync(int tagId)
    {
        var tag = await _tagRepository.GetByIdWithArticlesAsync(tagId);
        
        if (tag == null || !tag.NewsArticles.Any())
            return Enumerable.Empty<NewsArticleDto>();

        return tag.NewsArticles.Select(article => new NewsArticleDto
        {
            NewsArticleId = article.NewsArticleId,
            NewsTitle = article.NewsTitle,
            Headline = article.Headline,
            NewsContent = article.NewsContent,
            NewsSource = article.NewsSource,
            CategoryId = article.CategoryId,
            CategoryName = article.Category?.CategoryName,
            NewsStatus = article.NewsStatus,
            CreatedById = article.CreatedById,
            CreatedByName = article.CreatedBy?.AccountName,
            CreatedDate = article.CreatedDate,
            ModifiedDate = article.ModifiedDate,
            Tags = new List<TagDto>()
        }).ToList();
    }

    private TagDto MapToDto(Tag tag)
    {
        return new TagDto
        {
            TagId = tag.TagId,
            TagName = tag.TagName,
            Note = tag.Note,
            ArticleCount = tag.NewsArticles?.Count ?? 0
        };
    }
}
