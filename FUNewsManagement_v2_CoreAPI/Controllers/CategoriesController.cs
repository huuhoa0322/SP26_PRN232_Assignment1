using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers
{
    /// <summary>
    /// Controller cho Category Management (Staff)
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin,Staff")] // Staff and Admin can access
    public class CategoriesController : ODataController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// GET /odata/Categories - List categories
        /// </summary>
        [HttpGet("odata/Categories")]
        [EnableQuery(MaxTop = 100)]
        public async Task<IActionResult> Get()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// GET /api/categories/{id} - Get detail
        /// </summary>
        [HttpGet("api/categories/{id}")]
        public async Task<IActionResult> Get([FromRoute] short id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound(new { message = "Danh mục không tồn tại" });
            return Ok(category);
        }

        /// <summary>
        /// POST /api/categories - Create
        /// </summary>
        [HttpPost("api/categories")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var created = await _categoryService.CreateAsync(request);
                return CreatedAtAction(nameof(Get), new { id = created.CategoryId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/categories/{id} - Update
        /// </summary>
        [HttpPut("api/categories/{id}")]
        public async Task<IActionResult> Update([FromRoute] short id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var updated = await _categoryService.UpdateAsync(id, request);
                if (updated == null) return NotFound(new { message = "Danh mục không tồn tại" });
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/categories/{id} - Delete
        /// </summary>
        [HttpDelete("api/categories/{id}")]
        public async Task<IActionResult> Delete([FromRoute] short id)
        {
            try
            {
                var success = await _categoryService.DeleteAsync(id);
                if (!success) return NotFound(new { message = "Danh mục không tồn tại" });
                return Ok(new { message = "Xóa danh mục thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PATCH /api/categories/{id}/status - Toggle Active Status
        /// </summary>
        [HttpPatch("api/categories/{id}/status")]
        public async Task<IActionResult> ToggleStatus([FromRoute] short id)
        {
            var success = await _categoryService.ToggleStatusAsync(id);
            if (!success) return NotFound(new { message = "Danh mục không tồn tại" });
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }
    }
}
