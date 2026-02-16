using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllWithArticleCountAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(short id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
        {
            // Check duplicate name? 
            // Requirement 1: "Category names under the same parent should be unique."
            // Simple check: unique name globally for now or implement parent check later.
            // Let's implement global unique name for simplicity unless parent is specified.
            var existing = (await _categoryRepository.GetAllAsync())
                .FirstOrDefault(c => c.CategoryName.Equals(request.CategoryName, StringComparison.OrdinalIgnoreCase) 
                                     && c.ParentCategoryId == request.ParentCategoryId);
            
            if (existing != null)
            {
                throw new InvalidOperationException($"Danh mục '{request.CategoryName}' đã tồn tại trong cùng cấp cha.");
            }

            var category = _mapper.Map<Category>(request);
            await _categoryRepository.AddAsync(category);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto?> UpdateAsync(short id, UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            // Check if ParentCategoryId changes and if it's used by articles
            if (request.ParentCategoryId.HasValue && request.ParentCategoryId != category.ParentCategoryId)
            {
                // Requirement 1: "When editing, ParentCategoryID cannot be changed if the category is already used by articles."
                var isUsed = await _categoryRepository.IsCategoryUsedAsync(id);
                if (isUsed)
                {
                     throw new InvalidOperationException("Không thể thay đổi danh mục cha vì danh mục này đã có bài viết.");
                }
            }

            // Check duplicate name on update
            if (!string.IsNullOrEmpty(request.CategoryName) && request.CategoryName != category.CategoryName)
            {
                 var existing = (await _categoryRepository.GetAllAsync())
                    .FirstOrDefault(c => c.CategoryName.Equals(request.CategoryName, StringComparison.OrdinalIgnoreCase) 
                                         && c.ParentCategoryId == (request.ParentCategoryId ?? category.ParentCategoryId)
                                         && c.CategoryId != id);
                if (existing != null)
                {
                    throw new InvalidOperationException($"Danh mục '{request.CategoryName}' đã tồn tại.");
                }
            }
            
            _mapper.Map(request, category);
            await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var isUsed = await _categoryRepository.IsCategoryUsedAsync(id);
            if (isUsed)
            {
                throw new InvalidOperationException("Không thể xóa danh mục này vì đã có bài viết.");
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;
            await _categoryRepository.DeleteAsync(category);
            return true;
        }

        public async Task<bool> ToggleStatusAsync(short id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            category.IsActive = !category.IsActive; // Toggle
            await _categoryRepository.UpdateAsync(category);
            return true;
        }
    }
}
