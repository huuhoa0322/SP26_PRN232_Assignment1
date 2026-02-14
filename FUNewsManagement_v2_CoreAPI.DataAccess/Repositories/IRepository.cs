namespace FUNewsManagement_v2_CoreAPI.DataAccess.Repositories
{
    /// <summary>
    /// Generic repository interface cho tất cả entities
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Lấy tất cả records
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Lấy record theo ID
        /// </summary>
        Task<T?> GetByIdAsync(object id);

        /// <summary>
        /// Thêm mới record
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Cập nhật record
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Xóa record
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Kiểm tra record có tồn tại không
        /// </summary>
        Task<bool> ExistsAsync(object id);

        /// <summary>
        /// Lưu thay đổi vào database
        /// </summary>
        Task SaveChangesAsync();
    }
}
