using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.DTOs
{
    /// <summary>
    /// Flat record used for OData-queryable dashboard.
    /// FE groups/aggregates these rows with JS (e.g. Chart.js).
    /// </summary>
    public class DashboardItemDto
    {
        [Key]
        public string NewsArticleId { get; set; } = null!;
        public string? NewsTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? NewsStatus { get; set; }

        // Category info
        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        // Author info
        public short? CreatedById { get; set; }
        public string? AuthorName { get; set; }
    }
}
