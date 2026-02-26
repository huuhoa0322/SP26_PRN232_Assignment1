using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : BaseApiController
    {
        private readonly CoreApiService _apiService;
        private readonly ILogger<TagsController> _logger;

        public TagsController(CoreApiService apiService, ILogger<TagsController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags([FromQuery] string? filter, [FromQuery] string? orderby, [FromQuery] int? top, [FromQuery] int? skip)
        {
            try
            {
                // Check if session has token
                var token = HttpContext.Session.GetString("JwtToken");
                _logger.LogInformation($"GetTags called. Has token: {!string.IsNullOrEmpty(token)}");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No JWT token found in session");
                    return Unauthorized("No authentication token found");
                }

                var result = await _apiService.GetTagsAsync(filter, orderby, top, skip);
                if (result == null)
                    return StatusCode(500, "Failed to fetch tags");

                bool isOffline = HttpContext.Items.ContainsKey("IsOfflineMode") && (bool)HttpContext.Items["IsOfflineMode"];

                return Ok(new { value = result.Value, count = result.Count, isOffline = isOffline });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTags");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTag(int id)
        {
            try
            {
                var tag = await _apiService.GetTagByIdAsync(id);
                if (tag == null) return NotFound();
                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTag");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
        {
            try
            {
                var result = await _apiService.CreateTagAsync(request);
                if (result == null) return BadRequest("Failed to create tag");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateTag");
                if (IsOfflineException(ex)) return OfflineResult();
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagRequest request)
        {
            try
            {
                var result = await _apiService.UpdateTagAsync(id, request);
                if (result == null) return BadRequest("Failed to update tag");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateTag");
                if (IsOfflineException(ex)) return OfflineResult();
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var success = await _apiService.DeleteTagAsync(id);
                if (!success) return BadRequest("Failed to delete tag");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteTag");
                if (IsOfflineException(ex)) return OfflineResult();
                return BadRequest(ex.Message);
            }
        }
    }
}
