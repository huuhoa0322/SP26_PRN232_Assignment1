using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag
{
    public class CreateTagRequest
    {
        [Required(ErrorMessage = "Tên tag là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên tag không được vượt quá 50 ký tự")]
        public string TagName { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Ghi chú không được vượt quá 255 ký tự")]
        public string? Note { get; set; }
    }
}
