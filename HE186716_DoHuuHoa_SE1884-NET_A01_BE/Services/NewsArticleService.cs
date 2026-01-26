using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

public class NewsArticleService : INewsArticleService
{
    private readonly INewsArticleRepository _newsArticleRepository;
    private readonly ITagRepository _tagRepository;

    public NewsArticleService(INewsArticleRepository newsArticleRepository, ITagRepository tagRepository)
    {
        _newsArticleRepository = newsArticleRepository;
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<NewsArticleDto>> GetAllAsync()
    {
        var articles = await _newsArticleRepository.GetAllWithDetailsAsync();
        return articles.Select(MapToDto);
    }

    public async Task<IEnumerable<NewsArticleDto>> GetActiveArticlesAsync()
    {
        var articles = await _newsArticleRepository.GetActiveArticlesAsync();
        return articles.Select(MapToDto);
    }

    public async Task<NewsArticleDto?> GetByIdAsync(string id)
    {
        var article = await _newsArticleRepository.GetByIdWithDetailsAsync(id);
        return article == null ? null : MapToDto(article);
    }

    public async Task<IEnumerable<NewsArticleDto>> SearchAsync(NewsArticleSearchDto searchDto)
    {
        var articles = await _newsArticleRepository.SearchAsync(
            searchDto.Keyword,
            searchDto.CategoryId,
            searchDto.Status);

        // Filter by date range if provided
        if (searchDto.StartDate.HasValue || searchDto.EndDate.HasValue)
        {
            articles = articles.Where(a =>
                (!searchDto.StartDate.HasValue || a.CreatedDate >= searchDto.StartDate.Value) &&
                (!searchDto.EndDate.HasValue || a.CreatedDate <= searchDto.EndDate.Value));
        }

        return articles.Select(MapToDto);
    }

    public async Task<IEnumerable<NewsArticleDto>> FilterByDateRangeAsync(DateTime? startDate, DateTime? endDate)
    {
        var articles = await _newsArticleRepository.FilterByDateRangeAsync(startDate, endDate);
        return articles.Select(MapToDto);
    }

    public async Task<IEnumerable<NewsArticleDto>> GetByAuthorAsync(short createdById)
    {
        var articles = await _newsArticleRepository.GetByAuthorAsync(createdById);
        return articles.Select(MapToDto);
    }

    public async Task<IEnumerable<NewsArticleDto>> GetRelatedArticlesAsync(string articleId)
    {
        var article = await _newsArticleRepository.GetByIdWithDetailsAsync(articleId);
        if (article == null)
            return Enumerable.Empty<NewsArticleDto>();

        var relatedArticles = await _newsArticleRepository.GetRelatedArticlesAsync(
            articleId,
            article.CategoryId,
            3);

        return relatedArticles.Select(MapToDto);
    }

    public async Task<NewsArticleDto> CreateAsync(CreateNewsArticleDto dto, short createdById)
    {
        var newId = await _newsArticleRepository.GenerateNewIdAsync();

        var article = new NewsArticle
        {
            NewsArticleId = newId,
            NewsTitle = dto.NewsTitle,
            Headline = dto.Headline,
            NewsContent = dto.NewsContent,
            NewsSource = dto.NewsSource,
            CategoryId = dto.CategoryId,
            NewsStatus = dto.NewsStatus,
            CreatedById = createdById,
            CreatedDate = DateTime.Now,
            Tags = new List<Tag>()
        };

        // Add tags
        if (dto.TagIds != null && dto.TagIds.Any())
        {
            foreach (var tagId in dto.TagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(tagId);
                if (tag != null)
                    article.Tags.Add(tag);
            }
        }

        await _newsArticleRepository.AddAsync(article);

        // Reload with details
        var created = await _newsArticleRepository.GetByIdWithDetailsAsync(newId);
        return MapToDto(created!);
    }

    public async Task<NewsArticleDto?> UpdateAsync(string id, UpdateNewsArticleDto dto, short updatedById)
    {
        var article = await _newsArticleRepository.GetByIdWithDetailsAsync(id);
        if (article == null)
            return null;

        article.NewsTitle = dto.NewsTitle;
        article.Headline = dto.Headline;
        article.NewsContent = dto.NewsContent;
        article.NewsSource = dto.NewsSource;
        article.CategoryId = dto.CategoryId;
        if (dto.NewsStatus.HasValue)
            article.NewsStatus = dto.NewsStatus.Value;
        article.UpdatedById = updatedById;
        article.ModifiedDate = DateTime.Now;

        // Update tags
        article.Tags.Clear();
        if (dto.TagIds != null && dto.TagIds.Any())
        {
            foreach (var tagId in dto.TagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(tagId);
                if (tag != null)
                    article.Tags.Add(tag);
            }
        }

        await _newsArticleRepository.UpdateAsync(article);

        var updated = await _newsArticleRepository.GetByIdWithDetailsAsync(id);
        return MapToDto(updated!);
    }

    public async Task<(bool Success, string Message)> DeleteAsync(string id)
    {
        var article = await _newsArticleRepository.GetByIdWithDetailsAsync(id);
        if (article == null)
            return (false, "Không tìm thấy bài viết");

        // Clear tags first (handled by many-to-many relationship)
        article.Tags.Clear();
        await _newsArticleRepository.UpdateAsync(article);

        await _newsArticleRepository.DeleteAsync(article);
        return (true, "Xóa bài viết thành công"); 
    }

    public async Task<NewsArticleDto?> DuplicateAsync(string id, short createdById)
    {
        var original = await _newsArticleRepository.GetByIdWithDetailsAsync(id);
        if (original == null)
            return null;

        var newId = await _newsArticleRepository.GenerateNewIdAsync();

        var duplicate = new NewsArticle
        {
            NewsArticleId = newId,
            NewsTitle = original.NewsTitle + " (Copy)",
            Headline = original.Headline,
            NewsContent = original.NewsContent,
            NewsSource = original.NewsSource,
            CategoryId = original.CategoryId,
            NewsStatus = false, // Set as inactive by default
            CreatedById = createdById,
            CreatedDate = DateTime.Now,
            Tags = new List<Tag>(original.Tags)
        };

        await _newsArticleRepository.AddAsync(duplicate);

        var created = await _newsArticleRepository.GetByIdWithDetailsAsync(newId);
        return MapToDto(created!);
    }

    private NewsArticleDto MapToDto(NewsArticle article)
    {
        return new NewsArticleDto
        {
            NewsArticleId = article.NewsArticleId,
            NewsTitle = article.NewsTitle,
            Headline = article.Headline,
            CreatedDate = article.CreatedDate,
            NewsContent = article.NewsContent,
            NewsSource = article.NewsSource,
            CategoryId = article.CategoryId,
            CategoryName = article.Category?.CategoryName,
            NewsStatus = article.NewsStatus,
            CreatedById = article.CreatedById,
            CreatedByName = article.CreatedBy?.AccountName,
            UpdatedById = article.UpdatedById,
            ModifiedDate = article.ModifiedDate,
            Tags = article.Tags?.Select(t => new TagDto
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Note = t.Note
            }).ToList() ?? new List<TagDto>()
        };
    }
    public async Task<PagedResultDto<NewsArticleDto>> SearchPagedAsync(NewsArticleSearchDto searchDto, int pageIndex, int pageSize)
    {
        var articles = await _newsArticleRepository.SearchAsync(
            searchDto.Keyword,
            searchDto.CategoryId,
            searchDto.Status);

        // Filter by date range if provided - Note: Repository SearchAsync returns IEnumerable, ideally should be IQueryable
        // For now we filter in memory since Repository pattern here returns IEnumerable
        if (searchDto.StartDate.HasValue || searchDto.EndDate.HasValue)
        {
            articles = articles.Where(a =>
                (!searchDto.StartDate.HasValue || a.CreatedDate >= searchDto.StartDate.Value) &&
                (!searchDto.EndDate.HasValue || a.CreatedDate <= searchDto.EndDate.Value));
        }

        var totalCount = articles.Count();
        
        var pagedArticles = articles
            .OrderByDescending(a => a.CreatedDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = pagedArticles.Select(MapToDto).ToList();

        return new PagedResultDto<NewsArticleDto>(dtos, totalCount, pageIndex, pageSize);
    }
}

