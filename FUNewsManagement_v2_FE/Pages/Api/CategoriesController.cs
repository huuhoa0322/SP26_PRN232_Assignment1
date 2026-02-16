using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using System.Text.Json;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CoreApiService _apiService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(CoreApiService apiService, ILogger<CategoriesController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] string? filter, [FromQuery] string? orderby, [FromQuery] int? top, [FromQuery] int? skip)
        {
            try
            {
                // Check if session has token
                var token = HttpContext.Session.GetString("JwtToken");
                _logger.LogInformation($"GetCategories called. Has token: {!string.IsNullOrEmpty(token)}");

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No JWT token found in session");
                    return Unauthorized("No authentication token found");
                }

                var result = await _apiService.GetCategoriesAsync(filter, orderby, top, skip);
                if (result == null)
                    return StatusCode(500, "Failed to fetch categories");

                return Ok(new { value = result.Value, count = result.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCategories");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(short id)
        {
            try
            {
                var category = await _apiService.GetCategoryByIdAsync(id);
                if (category == null) return NotFound();
                return Ok(category);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error in GetCategory");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var result = await _apiService.CreateCategoryAsync(request);
                if (result == null) return BadRequest("Failed to create category");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCategory");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(short id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var result = await _apiService.UpdateCategoryAsync(id, request);
                if (result == null) return BadRequest("Failed to update category");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCategory");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(short id)
        {
            try
            {
                var success = await _apiService.DeleteCategoryAsync(id);
                if (!success) return BadRequest("Failed to delete category");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCategory");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleStatus(short id)
        {
            try
            {
                var success = await _apiService.ToggleCategoryStatusAsync(id);
                if (!success) return BadRequest("Failed to toggle status");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ToggleStatus");
                return BadRequest(ex.Message);
            }
        }
    }
}
