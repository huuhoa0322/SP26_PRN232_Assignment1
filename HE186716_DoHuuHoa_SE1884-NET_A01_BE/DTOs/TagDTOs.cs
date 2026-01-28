using System.ComponentModel.DataAnnotations;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;

// ===== TAG DTOs =====
public class TagDto
{
    public int TagId { get; set; }
    public string? TagName { get; set; } 
    public string? Note { get; set; }
    public int ArticleCount { get; set; }
}

public class CreateTagDto
{
    [Required(ErrorMessage = "Tên thẻ là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên thẻ không được vượt quá 50 ký tự")]
    public string TagName { get; set; } = null!;

    [StringLength(400, ErrorMessage = "Ghi chú không được vượt quá 400 ký tự")]
    public string? Note { get; set; }
}

public class UpdateTagDto
{
    [Required(ErrorMessage = "Tên thẻ là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên thẻ không được vượt quá 50 ký tự")]
    public string TagName { get; set; } = null!;

    [StringLength(400, ErrorMessage = "Ghi chú không được vượt quá 400 ký tự")] 
    public string? Note { get; set; }
}
