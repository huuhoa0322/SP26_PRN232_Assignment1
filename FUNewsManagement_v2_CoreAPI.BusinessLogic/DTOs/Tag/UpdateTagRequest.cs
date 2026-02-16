using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag
{
    public class UpdateTagRequest
    {
        [StringLength(50, ErrorMessage = "Tên tag không được vượt quá 50 ký tự")]
        public string? TagName { get; set; }

        [StringLength(255, ErrorMessage = "Ghi chú không được vượt quá 255 ký tự")]
        public string? Note { get; set; }
    }
}
