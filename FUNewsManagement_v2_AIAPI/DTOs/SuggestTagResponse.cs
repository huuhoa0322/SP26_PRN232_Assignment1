namespace FUNewsManagement_v2_AIAPI.DTOs
{
    public class SuggestTagResponse
    {
        public List<string> SuggestedTags { get; set; } = new List<string>();
        // Optional, included as per requirement "optionally with confidence scores"
        // Key: Tag, Value: Score (0-100)
        public Dictionary<string, int> TagConfidence { get; set; } = new Dictionary<string, int>(); 
    }
}
