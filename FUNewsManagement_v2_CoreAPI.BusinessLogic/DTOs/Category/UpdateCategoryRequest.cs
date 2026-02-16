using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category
{
    public class UpdateCategoryRequest
    {
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string? CategoryName { get; set; }

        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        public string? CategoryDesciption { get; set; }

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }
}
