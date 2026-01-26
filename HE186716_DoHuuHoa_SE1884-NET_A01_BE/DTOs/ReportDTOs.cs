using System.ComponentModel.DataAnnotations;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;

// ===== REPORT DTOs =====
public class ReportFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

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
    public string CategoryName { get; set; } = null!;
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
