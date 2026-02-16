using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News
{
    public class UpdateNewsArticleRequest
    {
        [StringLength(400, ErrorMessage = "Tiêu đề không quá 400 ký tự")]
        public string? NewsTitle { get; set; }

        [StringLength(150, ErrorMessage = "Headline không quá 150 ký tự")]
        public string? Headline { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; }

        public List<int>? TagIds { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
