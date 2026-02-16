namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag
{
    public class TagDto
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
        public int NewsCount { get; set; }
    }
}
