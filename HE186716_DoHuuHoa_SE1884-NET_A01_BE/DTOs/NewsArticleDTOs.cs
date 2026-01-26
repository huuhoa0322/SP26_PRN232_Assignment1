using System.ComponentModel.DataAnnotations;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;

// ===== NEWS ARTICLE DTOs =====
public class NewsArticleDto
{
    public string NewsArticleId { get; set; } = null!;
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = null!;
    public DateTime? CreatedDate { get; set; }
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool? NewsStatus { get; set; }
    public short? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public short? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public List<TagDto> Tags { get; set; } = new();
}

public class CreateNewsArticleDto
{
    [StringLength(400, ErrorMessage = "Tiêu đề không được vượt quá 400 ký tự")]
    public string? NewsTitle { get; set; }

    [Required(ErrorMessage = "Tiêu đề ngắn là bắt buộc")]
    [StringLength(150, ErrorMessage = "Tiêu đề ngắn không được vượt quá 150 ký tự")]
    public string Headline { get; set; } = null!;

    [StringLength(20000, ErrorMessage = "Nội dung không được vượt quá 20000 ký tự")]
    public string? NewsContent { get; set; }

    [StringLength(400, ErrorMessage = "Nguồn không được vượt quá 400 ký tự")]
    public string? NewsSource { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]  
    public short CategoryId { get; set; }

    public bool NewsStatus { get; set; } = true;

    public List<int>? TagIds { get; set; }
}

public class UpdateNewsArticleDto
{
    [StringLength(400, ErrorMessage = "Tiêu đề không được vượt quá 400 ký tự")]
    public string? NewsTitle { get; set; }

    [Required(ErrorMessage = "Tiêu đề ngắn là bắt buộc")]
    [StringLength(150, ErrorMessage = "Tiêu đề ngắn không được vượt quá 150 ký tự")]
    public string Headline { get; set; } = null!;

    [StringLength(4000, ErrorMessage = "Nội dung không được vượt quá 4000 ký tự")]
    public string? NewsContent { get; set; }

    [StringLength(400, ErrorMessage = "Nguồn không được vượt quá 400 ký tự")]
    public string? NewsSource { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    public short CategoryId { get; set; }

    public bool? NewsStatus { get; set; }

    public List<int>? TagIds { get; set; }
}

public class NewsArticleSearchDto
{
    public string? Keyword { get; set; }
    public short? CategoryId { get; set; }
    public bool? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
