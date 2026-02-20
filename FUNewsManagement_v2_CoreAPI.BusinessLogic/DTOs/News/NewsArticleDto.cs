using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News
{
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
        public string? ImageUrl { get; set; }
        
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}
