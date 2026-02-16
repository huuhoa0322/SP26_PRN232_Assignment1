using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.TagName == name);
        }

        public async Task<bool> IsTagUsedAsync(int id)
        {
            // Check if any NewsArticle has this Tag
            return await _context.NewsArticles.AnyAsync(n => n.Tags.Any(t => t.TagId == id));
        }
    }
}
