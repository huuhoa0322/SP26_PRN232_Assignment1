using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.DTOs
{
    /// <summary>
    /// Represents a trending article.
    /// Trending = active articles ordered by recency, with tag count as a hotness indicator.
    /// </summary>
    public class TrendingArticleDto
    {
        [Key]
        public string NewsArticleId { get; set; } = null!;
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public int TagCount { get; set; }
    }
}
