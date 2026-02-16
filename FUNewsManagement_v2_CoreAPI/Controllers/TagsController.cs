using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers
{
    /// <summary>
    /// Controller cho Tag Management (Staff)
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin,Staff")] // Staff and Admin can access
    public class TagsController : ODataController
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// GET /odata/Tags - List
        /// </summary>
        [HttpGet("odata/Tags")]
        [EnableQuery(MaxTop = 100)]
        public async Task<IActionResult> Get()
        {
            var tags = await _tagService.GetAllAsync();
            return Ok(tags);
        }

        /// <summary>
        /// GET /api/tags/{id} - Detail
        /// </summary>
        [HttpGet("api/tags/{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null) return NotFound(new { message = "Tag không tồn tại" });
            return Ok(tag);
        }

        /// <summary>
        /// POST /api/tags - Create
        /// </summary>
        [HttpPost("api/tags")]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            try
            {
                var created = await _tagService.CreateAsync(request);
                return CreatedAtAction(nameof(Get), new { id = created.TagId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/tags/{id} - Update
        /// </summary>
        [HttpPut("api/tags/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTagRequest request)
        {
            try
            {
                var updated = await _tagService.UpdateAsync(id, request);
                if (updated == null) return NotFound(new { message = "Tag không tồn tại" });
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/tags/{id} - Delete
        /// </summary>
        [HttpDelete("api/tags/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var success = await _tagService.DeleteAsync(id);
                if (!success) return NotFound(new { message = "Tag không tồn tại" });
                return Ok(new { message = "Xóa Tag thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
