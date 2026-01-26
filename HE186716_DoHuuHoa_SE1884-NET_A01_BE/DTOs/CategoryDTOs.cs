using System.ComponentModel.DataAnnotations;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;

// ===== CATEGORY DTOs =====
public class CategoryDto
{
    public short CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryDesciption { get; set; } = null!;
    public short? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public bool? IsActive { get; set; }
    public int ArticleCount { get; set; }
}

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
    public string CategoryName { get; set; } = null!;

    [Required(ErrorMessage = "Mô tả danh mục là bắt buộc")]
    [StringLength(250, ErrorMessage = "Mô tả không được vượt quá 250 ký tự")] 
    public string CategoryDesciption { get; set; } = null!;

    public short? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
    public string CategoryName { get; set; } = null!;

    [Required(ErrorMessage = "Mô tả danh mục là bắt buộc")]
    [StringLength(250, ErrorMessage = "Mô tả không được vượt quá 250 ký tự")]
    public string CategoryDesciption { get; set; } = null!;

    public short? ParentCategoryId { get; set; }

    public bool? IsActive { get; set; }
}
