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

// ===== ACCOUNT DTOs =====
public class AccountDto
{
    public short AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountEmail { get; set; }
    public int? AccountRole { get; set; }
    public string RoleName { get; set; } = null!;
    public int ArticleCount { get; set; }
}

public class CreateAccountDto
{
    public string AccountName { get; set; } = null!;
    public string AccountEmail { get; set; } = null!;
    public string AccountPassword { get; set; } = null!;
    public int AccountRole { get; set; }
}

public class UpdateAccountDto
{
    public string AccountName { get; set; } = null!;
    public string AccountEmail { get; set; } = null!;
    public string? AccountPassword { get; set; }
    public int AccountRole { get; set; }
}

// ===== REPORT DTOs =====
public class ReportStatisticsDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int TotalArticles { get; set; }
    public int ActiveArticles { get; set; }
    public int InactiveArticles { get; set; }
    public List<CategoryStatDto> CategoryStats { get; set; } = new();
    public List<AuthorStatDto> AuthorStats { get; set; } = new();
    public List<StatusStatDto> StatusStats { get; set; } = new();
}

public class CategoryStatDto
{
    public short CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int ArticleCount { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
}

public class AuthorStatDto
{
    public short AccountId { get; set; }
    public string? AccountName { get; set; }
    public int ArticleCount { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
}

public class StatusStatDto
{
    public bool Status { get; set; }
    public string StatusName { get; set; } = null!;
    public int ArticleCount { get; set; }
    public DateTime? LatestCreatedDate { get; set; }
}

// ===== CATEGORY CREATE/UPDATE DTOs =====
public class CreateCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public string CategoryDesciption { get; set; } = null!;
    public short? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public string CategoryDesciption { get; set; } = null!;
    public short? ParentCategoryId { get; set; }
    public bool? IsActive { get; set; }
}

// ===== NEWS ARTICLE CREATE/UPDATE DTOs =====
public class CreateNewsArticleDto
{
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = null!;
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryId { get; set; }
    public bool NewsStatus { get; set; } = true;
    public List<int>? TagIds { get; set; }
}

public class UpdateNewsArticleDto
{
    public string? NewsTitle { get; set; }
    public string Headline { get; set; } = null!;
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryId { get; set; }
    public bool? NewsStatus { get; set; }
    public List<int>? TagIds { get; set; }
}

// ===== TAG CREATE/UPDATE DTOs =====
public class CreateTagDto
{
    public string TagName { get; set; } = null!;
    public string? Note { get; set; }
}

public class UpdateTagDto
{
    public string TagName { get; set; } = null!;
    public string? Note { get; set; }
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}



