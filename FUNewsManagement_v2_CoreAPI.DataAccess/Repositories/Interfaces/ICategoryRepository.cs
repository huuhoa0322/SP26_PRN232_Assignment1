using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> IsCategoryUsedAsync(short id);
        Task<List<Category>> GetAllWithArticleCountAsync();
    }
}
