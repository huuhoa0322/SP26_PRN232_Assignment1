using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(short id);
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
        Task<CategoryDto?> UpdateAsync(short id, UpdateCategoryRequest request);
        Task<bool> DeleteAsync(short id);
        Task<bool> ToggleStatusAsync(short id);
    }
}
