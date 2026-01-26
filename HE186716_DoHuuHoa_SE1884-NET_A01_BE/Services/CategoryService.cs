using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Models;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Repositories;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllWithArticleCountAsync();
        return categories.Select(MapToDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _categoryRepository.GetActiveCategoriesAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(short id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : MapToDto(category);
    }

    public async Task<IEnumerable<CategoryDto>> SearchAsync(string? keyword)
    {
        var categories = await _categoryRepository.SearchAsync(keyword);
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            CategoryName = dto.CategoryName,
            CategoryDesciption = dto.CategoryDesciption,
            ParentCategoryId = dto.ParentCategoryId,
            IsActive = dto.IsActive
        };

        await _categoryRepository.AddAsync(category);
        return MapToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(short id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return null;

        // REQ 4.3: Check if ParentCategoryID is being changed and category has articles
        if (dto.ParentCategoryId != category.ParentCategoryId)
        {
            if (await _categoryRepository.HasArticlesAsync(id))
                throw new InvalidOperationException("Không thể thay đổi danh mục cha của danh mục đã có bài viết");  
        }

        category.CategoryName = dto.CategoryName;
        category.CategoryDesciption = dto.CategoryDesciption;
        category.ParentCategoryId = dto.ParentCategoryId;
        if (dto.IsActive.HasValue)
            category.IsActive = dto.IsActive.Value;

        await _categoryRepository.UpdateAsync(category);
        return MapToDto(category);
    }

    public async Task<(bool Success, string Message)> DeleteAsync(short id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return (false, "Không tìm thấy danh mục");

        // Check if category has any articles
        if (await _categoryRepository.HasArticlesAsync(id))
            return (false, "Không thể xóa danh mục có chứa bài viết");

        await _categoryRepository.DeleteAsync(category);
        return (true, "Xóa danh mục thành công");
    }

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryDesciption = category.CategoryDesciption,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.CategoryName,
            IsActive = category.IsActive,
            ArticleCount = category.NewsArticles?.Count ?? 0
        };
    }
}
