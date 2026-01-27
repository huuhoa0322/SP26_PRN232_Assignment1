using Microsoft.EntityFrameworkCore;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Models;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    public TagRepository(FunewsManagementContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tag>> GetAllWithArticlesAsync()
    {
        return await _dbSet.Include(t => t.NewsArticles).ToListAsync();
    }

    public async Task<IEnumerable<Tag>> SearchAsync(string? keyword)
    {
        var query = _dbSet.Include(t => t.NewsArticles).AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.ToLower();
            query = query.Where(t => 
                (t.TagName != null && t.TagName.ToLower().Contains(keyword)) ||
                (t.Note != null && t.Note.ToLower().Contains(keyword)));
        }

        return await query.ToListAsync();
    }

    public async Task<Tag?> GetByIdWithArticlesAsync(int tagId)
    {
        return await _dbSet
            .Include(t => t.NewsArticles)
                .ThenInclude(a => a.Category)
            .Include(t => t.NewsArticles)
                .ThenInclude(a => a.CreatedBy)
            .FirstOrDefaultAsync(t => t.TagId == tagId);
    }

    public async Task<bool> IsUsedInArticlesAsync(int tagId)
    {
        var tag = await _dbSet
            .Include(t => t.NewsArticles)
            .FirstOrDefaultAsync(t => t.TagId == tagId);

        return tag?.NewsArticles.Any() ?? false;
    }

    public async Task<bool> CheckTagNameExistsAsync(string tagName, int? excludeTagId = null)
    {
        var query = _dbSet.Where(t => t.TagName != null && t.TagName.ToLower() == tagName.ToLower());
        
        if (excludeTagId.HasValue)
        {
            query = query.Where(t => t.TagId != excludeTagId.Value);
        }
        
        return await query.AnyAsync(); 
    }

    public async Task<IEnumerable<Tag>> GetTagsByArticleIdAsync(string articleId)
    {
        var article = await _context.NewsArticles
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.NewsArticleId == articleId);

        return article?.Tags ?? Enumerable.Empty<Tag>();
    }

    public async Task<int> GenerateNewIdAsync()
    {
        var maxId = await _dbSet.MaxAsync(t => (int?)t.TagId) ?? 0;
        return maxId + 1;
    }
}
