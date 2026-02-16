using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllAsync();
        Task<TagDto?> GetByIdAsync(int id);
        Task<TagDto> CreateAsync(CreateTagRequest request);
        Task<TagDto?> UpdateAsync(int id, UpdateTagRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
