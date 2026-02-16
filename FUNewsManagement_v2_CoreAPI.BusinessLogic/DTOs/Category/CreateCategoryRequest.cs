using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự")]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }
        
        public bool? IsActive { get; set; } = true;
    }
}
