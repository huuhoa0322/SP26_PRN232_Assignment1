using FUNewsManagement_v2_AIAPI.DTOs;
using FUNewsManagement_v2_AIAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement_v2_AIAPI.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController : ControllerBase
    {
        private readonly ITagSuggestionService _tagSuggestionService;
        private readonly ILearningCacheService _learningCacheService;

        public AIController(ITagSuggestionService tagSuggestionService, ILearningCacheService learningCacheService)
        {
            _tagSuggestionService = tagSuggestionService;
            _learningCacheService = learningCacheService;
        }

        [HttpPost("suggest-tags")]
        public async Task<IActionResult> SuggestTags([FromBody] SuggestTagRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Content))
            {
                return BadRequest("Content cannot be empty.");
            }

            var response = await _tagSuggestionService.SuggestTagsAsync(request.Content);
            return Ok(response);
        }

        [HttpPost("learn-tags")]
        public IActionResult LearnTags([FromBody] LearnTagRequest request)
        {
            if (request?.SelectedTags == null || request.SelectedTags.Count == 0)
            {
                return BadRequest("No tags to learn.");
            }

            _learningCacheService.LearnTags(request.SelectedTags);
            return Ok(new { message = "Tags learned successfully" });
        }
    }
}
