using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    [ApiController]
    [Route("api/proxy/ai")]
    public class AIController : ControllerBase
    {
        private readonly CoreApiService _coreApiService;

        public AIController(CoreApiService coreApiService)
        {
            _coreApiService = coreApiService;
        }

        [HttpPost("suggest-tags")]
        public async Task<IActionResult> SuggestTags([FromBody] SuggestTagRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Content is required.");
            }

            var result = await _coreApiService.SuggestTagsAsync(request.Content);
            if (result == null)
            {
                return StatusCode(500, "Failed to get tag suggestions from AI API.");
            }

            return Ok(result);
        }

        [HttpPost("learn-tags")]
        public async Task<IActionResult> LearnTags([FromBody] LearnTagRequest request)
        {
            if (request.SelectedTags == null || !request.SelectedTags.Any())
            {
                 return BadRequest("No tags to learn.");
            }

            var success = await _coreApiService.LearnTagsAsync(request.SelectedTags);
            if (!success)
            {
                return StatusCode(500, "Failed to send learn signal to AI API.");
            }

            return Ok(new { message = "Tags learned successfully" });
        }
    }

    public class SuggestTagRequest
    {
        public string Content { get; set; } = string.Empty;
    }

    public class LearnTagRequest
    {
        public List<string> SelectedTags { get; set; } = new List<string>();
    }
}
