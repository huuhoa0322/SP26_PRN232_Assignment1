using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Security.Claims;

namespace FUNewsManagement_v2_CoreAPI.Controllers
{
    /// <summary>
    /// Controller cho News Article Management (Staff)
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin,Staff")] // Staff and Admin can access
    public class NewsController : ODataController
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        /// <summary>
        /// GET /api/news/{id} - Detail
        /// </summary>
        [HttpGet("api/news/{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var article = await _newsService.GetByIdAsync(id);
            if (article == null) return NotFound(new { message = "Bài viết không tồn tại" });
            return Ok(article);
        }

        /// <summary>
        /// POST /api/news - Create
        /// Note: Use [FromForm] to handle file upload along with other fields
        /// </summary>
        [HttpPost("api/news")]
        public async Task<IActionResult> Create([FromForm] CreateNewsArticleRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var created = await _newsService.CreateAsync(request, userId);
                return CreatedAtAction(nameof(Get), new { id = created.NewsArticleId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/news/{id} - Update
        /// </summary>
        [HttpPut("api/news/{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromForm] UpdateNewsArticleRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updated = await _newsService.UpdateAsync(id, request, userId);
                if (updated == null) return NotFound(new { message = "Bài viết không tồn tại" });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/news/{id} - Delete
        /// </summary>
        [HttpDelete("api/news/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                var success = await _newsService.DeleteAsync(id);
                if (!success) return NotFound(new { message = "Bài viết không tồn tại" });
                return Ok(new { message = "Xóa bài viết thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/news/{id}/duplicate - Duplicate
        /// </summary>
        [HttpPost("api/news/{id}/duplicate")]
        public async Task<IActionResult> Duplicate([FromRoute] string id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var duplicated = await _newsService.DuplicateAsync(id, userId);
                if (duplicated == null) return NotFound(new { message = "Bài viết gốc không tồn tại" });
                return CreatedAtAction(nameof(Get), new { id = duplicated.NewsArticleId }, duplicated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Duplicate failed: " + ex.Message });
            }
        }

        private short GetCurrentUserId()
        {
            var userIdString = User.FindFirst("AccountId")?.Value;
            if (short.TryParse(userIdString, out short userId))
            {
                return userId;
            }
            // Fallback or throw
            throw new UnauthorizedAccessException("User ID not found in token");
        }
    }
}
