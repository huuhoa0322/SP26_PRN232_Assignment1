using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers.OData
{
    [AllowAnonymous]
    public class TagsController : ODataController
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;

        public TagsController(ITagService tagService, ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        [EnableQuery(MaxTop = 100)]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var tags = await _tagService.GetAllAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OData GetTags");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
