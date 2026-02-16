using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces
{
    public interface INewsArticleRepository : IRepository<NewsArticle>
    {
        Task<NewsArticle?> GetByIdWithDetailsAsync(string id);
    }
}
