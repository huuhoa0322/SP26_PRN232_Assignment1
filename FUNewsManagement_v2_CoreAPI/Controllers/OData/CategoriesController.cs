using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers.OData
{
    [Authorize]
    public class CategoriesController : ODataController
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [EnableQuery(MaxTop = 100)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OData GetCategories");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
