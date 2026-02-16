using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FunewsManagementContext context) : base(context)
        {
        }

        // Override GetAllAsync to include related entities
        public override async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.NewsArticles)
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<bool> IsCategoryUsedAsync(short id)
        {
            return await _context.NewsArticles.AnyAsync(n => n.CategoryId == id);
        }

        public async Task<List<Category>> GetAllWithArticleCountAsync()
        {
            // Note: This might be complex with Generic Repository if we want DTO projection here,
            // but for OData we usually return Entities and let OData handle it, or return DTOs from Service.
            // For now, let's just return Categories with Include if needed, or rely on OData $expand.
            // However, the requirement mentions "Display number of articles". 
            // We can handle this in Service -> DTO mapping.
            return await _context.Categories
                .Include(c => c.NewsArticles)
                .Include(c => c.ParentCategory)
                .ToListAsync();
        }
    }
}
