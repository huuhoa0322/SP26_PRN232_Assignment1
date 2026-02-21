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
                .Include(n => n.UpdatedBy)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<NewsArticle?> GetByIdWithDetailsAsync(string id)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .Include(n => n.CreatedBy)
                .Include(n => n.UpdatedBy)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }

        /// <summary>
        /// Returns next sequential numeric ID: finds the maximum numeric value among all
        /// existing IDs (ignoring non-numeric ones) and returns max+1.
        /// Starts at 1 if no records exist.
        /// </summary>
        public async Task<string> GetNextIdAsync()
        {
            var ids = await _context.NewsArticles
                .Select(n => n.NewsArticleId)
                .ToListAsync();

            long max = 0;
            foreach (var id in ids)
            {
                if (long.TryParse(id, out var numericId) && numericId > max)
                    max = numericId;
            }

            return (max + 1).ToString();
        }

        // Note: For OData list, we rely on [EnableQuery] in controller to handle includes via $expand
    }
}
