using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
    {
        public NewsArticleRepository(FunewsManagementContext context) : base(context)
        {
        }

        // Override GetAllAsync to include related entities
        public override async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .Include(n => n.CreatedBy)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<NewsArticle?> GetByIdWithDetailsAsync(string id)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .Include(n => n.CreatedBy)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }

        // Note: For OData list, we rely on [EnableQuery] in controller to handle includes via $expand
    }
}
