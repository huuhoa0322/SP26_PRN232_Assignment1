using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces
{
    /// <summary>
    /// Interface cho Account management service
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Lấy tất cả accounts (for OData)
        /// </summary>
        Task<IEnumerable<AccountDto>> GetAllAsync(); 

        /// <summary>
        /// Lấy account theo ID
        /// </summary>
        Task<AccountDto?> GetByIdAsync(short id);

        /// <summary>
        /// Tạo mới account
        /// </summary>
        Task<AccountDto> CreateAsync(CreateAccountRequest request);

        /// <summary>
        /// Cập nhật account
        /// </summary>
        Task<AccountDto?> UpdateAsync(short id, UpdateAccountRequest request);

        /// <summary>
        /// Xóa account
        /// </summary>
        Task<bool> DeleteAsync(short id);

        /// <summary>
        /// Kiểm tra email đã tồn tại
        /// </summary>
        Task<bool> EmailExistsAsync(string email, short? excludeId = null);
    }
}
