using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagement_v2_CoreAPI.Controllers.OData
{
    [Authorize]
    public class NewsArticlesController : ODataController
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsArticlesController> _logger;

        public NewsArticlesController(INewsService newsService, ILogger<NewsArticlesController> logger)
        {
            _newsService = newsService;
            _logger = logger;
        }

        [EnableQuery(MaxTop = 100, MaxExpansionDepth = 3)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation("OData NewsArticles GET called");
                var news = await _newsService.GetAllAsync();
                _logger.LogInformation($"Returning {news.Count()} news articles");
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OData GetNewsArticles");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
