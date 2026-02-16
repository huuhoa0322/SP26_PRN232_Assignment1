using FUNewsManagement_v2_FE.Services;
using Microsoft.AspNetCore.Mvc;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;

namespace FUNewsManagement_v2_FE.Pages.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly CoreApiService _apiService;
        private readonly ILogger<NewsController> _logger;

        public NewsController(CoreApiService apiService, ILogger<NewsController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetNews([FromQuery] string? filter, [FromQuery] string? orderby, [FromQuery] int? top, [FromQuery] int? skip)
        {
            try
            {
                var result = await _apiService.GetNewsArticlesAsync(filter, orderby, top, skip);
                if (result == null)
                    return StatusCode(500, "Failed to fetch news articles");

                return Ok(new { value = result.Value, count = result.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetNews");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsArticle(string id)
        {
            try
            {
                var article = await _apiService.GetNewsArticleByIdAsync(id);
                if (article == null) return NotFound();
                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetNewsArticle");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNews([FromForm] NewsArticleCreateViewModel model)
        {
            try
            {
                var request = new CreateNewsArticleRequest
                {
                    NewsTitle = model.NewsTitle,
                    Headline = model.Headline,
                    NewsContent = model.NewsContent,
                    NewsSource = model.NewsSource,
                    CategoryId = model.CategoryId,
                    NewsStatus = model.NewsStatus,
                    TagIds = model.TagIds
                };

                Stream? imageStream = null;
                string? imageName = null;

                if (model.ImageFile != null)
                {
                    imageStream = model.ImageFile.OpenReadStream();
                    imageName = model.ImageFile.FileName;
                }

                using (imageStream)
                {
                    var result = await _apiService.CreateNewsArticleAsync(request, imageStream, imageName);
                    if (result == null) return BadRequest("Failed to create news article");
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNews");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(string id, [FromForm] NewsArticleUpdateViewModel model)
        {
            try
            {
                var request = new UpdateNewsArticleRequest
                {
                    NewsTitle = model.NewsTitle,
                    Headline = model.Headline,
                    NewsContent = model.NewsContent,
                    NewsSource = model.NewsSource,
                    CategoryId = model.CategoryId,
                    NewsStatus = model.NewsStatus,
                    TagIds = model.TagIds
                };

                Stream? imageStream = null;
                string? imageName = null;

                if (model.ImageFile != null)
                {
                    imageStream = model.ImageFile.OpenReadStream();
                    imageName = model.ImageFile.FileName;
                }

                using (imageStream)
                {
                    var result = await _apiService.UpdateNewsArticleAsync(id, request, imageStream, imageName);
                    if (result == null) return BadRequest("Failed to update news article");
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateNews");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(string id)
        {
            try
            {
                var success = await _apiService.DeleteNewsArticleAsync(id);
                if (!success) return BadRequest("Failed to delete news article");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteNews");
                return BadRequest(ex.Message);
            }
        }
    }

    public class NewsArticleCreateViewModel
    {
        public string NewsTitle { get; set; } = null!;
        public string Headline { get; set; } = null!;
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short CategoryId { get; set; }
        public bool NewsStatus { get; set; } = true;
        public List<int>? TagIds { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class NewsArticleUpdateViewModel
    {
        public string? NewsTitle { get; set; }
        public string? Headline { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
        public List<int>? TagIds { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
