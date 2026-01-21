namespace HE186716_DoHuuHoa_SE1884_NET_A01_FE.Models;

// ===== AUTH DTOs =====
public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponse
{
    public short AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountEmail { get; set; }
    public int? AccountRole { get; set; }
    public string RoleName { get; set; } = null!;
    public bool IsAdmin { get; set; }
}

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
    public List<TagDto> Tags { get; set; } = new();
}

// ===== CATEGORY DTOs =====
public class CategoryDto
{
    public short CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryDesciption { get; set; } = null!;
    public bool? IsActive { get; set; }
    public int ArticleCount { get; set; }
}

// ===== TAG DTOs =====
public class TagDto
{
    public int TagId { get; set; }
    public string? TagName { get; set; }
    public string? Note { get; set; }
}
