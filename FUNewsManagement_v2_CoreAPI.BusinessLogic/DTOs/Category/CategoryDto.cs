namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category
{
    public class CategoryDto
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CategoryDesciption { get; set; } = null!;
        public short? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool? IsActive { get; set; }
        public int? NewsCount { get; set; } // For display
    }
}
