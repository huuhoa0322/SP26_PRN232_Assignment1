using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // For IFormFile

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News
{
    public class CreateNewsArticleRequest
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(400, ErrorMessage = "Tiêu đề không quá 400 ký tự")]
        public string NewsTitle { get; set; } = null!;

        [Required(ErrorMessage = "Headline là bắt buộc")]
        [StringLength(150, ErrorMessage = "Headline không quá 150 ký tự")]
        public string Headline { get; set; } = null!;

        public string? NewsContent { get; set; }
        
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public short CategoryId { get; set; }

        public bool NewsStatus { get; set; } = true;

        public List<int>? TagIds { get; set; }
        
        // Handling Image Upload separately or base64? 
        // Requirement mentions "Allow image uploads for articles".
        // API usually takes Multipart/Form-Data. 
        // However, DTO binding with IFormFile in nested JSON is tricky.
        // We will separate Image Upload or use [FromForm].
        // Let's use [FromForm] in Controller, mapping to this DTO.
        // But for DTO to hold file it needs IFormFile.
        // Since we are using CreateMap, we might need a separate model or mapping logic.
        // Let's add IFormFile here, but ignore it in AutoMapper and handle manually in Service.
        public IFormFile? ImageFile { get; set; }
    }
}
