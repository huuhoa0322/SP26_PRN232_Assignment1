using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces
{
    public interface INewsArticleRepository : IRepository<NewsArticle>
    {
        Task<NewsArticle?> GetByIdWithDetailsAsync(string id);
        /// <summary>Returns the next sequential numeric ID as a string (max existing + 1).</summary>
        Task<string> GetNextIdAsync();
    }
}
