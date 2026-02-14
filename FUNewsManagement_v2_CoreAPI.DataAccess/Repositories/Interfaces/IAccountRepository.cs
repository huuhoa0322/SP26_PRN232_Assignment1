using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface cho SystemAccount entity
    /// </summary>
    public interface IAccountRepository : IRepository<SystemAccount>
    {
        /// <summary>
        /// Lấy account theo email
        /// </summary>
        Task<SystemAccount?> GetByEmailAsync(string email);

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Tìm kiếm accounts theo filters
        /// </summary>
        Task<IEnumerable<SystemAccount>> SearchAsync(string? name, string? email, short? role);
    }
}
