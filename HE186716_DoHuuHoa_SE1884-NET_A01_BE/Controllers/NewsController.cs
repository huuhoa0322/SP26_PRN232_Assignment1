using Microsoft.AspNetCore.Mvc;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NewsController : ControllerBase
{
    private readonly INewsArticleService _newsArticleService;

    public NewsController(INewsArticleService newsArticleService)
    {
        _newsArticleService = newsArticleService;
    }

    /// <summary>
    /// Get all news articles (for staff/admin)
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<NewsArticleDto>>> GetAll()
    {
        var articles = await _newsArticleService.GetAllAsync();
        return Ok(articles);
    }

    /// <summary>
    /// Get active news articles only (public)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsArticleDto>>> GetActive()
    {
        var articles = await _newsArticleService.GetActiveArticlesAsync();
        return Ok(articles);
    }

    /// <summary>
    /// Get news article by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<NewsArticleDto>> GetById(string id)
    {
        var article = await _newsArticleService.GetByIdAsync(id);
        if (article == null)
            return NotFound(new { message = "Article not found" });

        return Ok(article);
    }

    /// <summary>
    /// Search news articles
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<NewsArticleDto>>> Search([FromQuery] NewsArticleSearchDto searchDto)
    {
        var articles = await _newsArticleService.SearchAsync(searchDto);
        return Ok(articles);
    }

    /// <summary>
    /// Filter news articles by date range
    /// </summary>
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<NewsArticleDto>>> Filter(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var articles = await _newsArticleService.FilterByDateRangeAsync(startDate, endDate);
        return Ok(articles);
    }

    /// <summary>
    /// Get articles by author
    /// </summary>
    [HttpGet("author/{authorId}")]
    public async Task<ActionResult<IEnumerable<NewsArticleDto>>> GetByAuthor(short authorId)
    {
        var articles = await _newsArticleService.GetByAuthorAsync(authorId);
        return Ok(articles);
    }

    /// <summary>
    /// Get related articles
    /// </summary>
    [HttpGet("{id}/related")]
    public async Task<ActionResult<IEnumerable<NewsArticleDto>>> GetRelated(string id)
    {
        var articles = await _newsArticleService.GetRelatedArticlesAsync(id);
        return Ok(articles);
    }

    /// <summary>
    /// Create a new news article
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<NewsArticleDto>> Create(
        [FromBody] CreateNewsArticleDto dto,
        [FromQuery] short createdById)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var article = await _newsArticleService.CreateAsync(dto, createdById);
        return CreatedAtAction(nameof(GetById), new { id = article.NewsArticleId }, article);
    }

    /// <summary>
    /// Update an existing news article
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<NewsArticleDto>> Update(
        string id,
        [FromBody] UpdateNewsArticleDto dto,
        [FromQuery] short updatedById)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var article = await _newsArticleService.UpdateAsync(id, dto, updatedById);
        if (article == null)
            return NotFound(new { message = "Article not found" });

        return Ok(article);
    }

    /// <summary>
    /// Delete a news article
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _newsArticleService.DeleteAsync(id);
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    /// <summary>
    /// Duplicate a news article
    /// </summary>
    [HttpPost("{id}/duplicate")]
    public async Task<ActionResult<NewsArticleDto>> Duplicate(string id, [FromQuery] short createdById)
    {
        var article = await _newsArticleService.DuplicateAsync(id, createdById);
        if (article == null)
            return NotFound(new { message = "Original article not found" });

        return CreatedAtAction(nameof(GetById), new { id = article.NewsArticleId }, article);
    }
}
