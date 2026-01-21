using System.Text;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Repositories;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

public class ReportService : IReportService
{
    private readonly INewsArticleRepository _newsArticleRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAccountRepository _accountRepository;

    public ReportService(
        INewsArticleRepository newsArticleRepository,
        ICategoryRepository categoryRepository,
        IAccountRepository accountRepository)
    {
        _newsArticleRepository = newsArticleRepository;
        _categoryRepository = categoryRepository;
        _accountRepository = accountRepository;
    }

    public async Task<ReportStatisticsDto> GetStatisticsAsync(ReportFilterDto filter)
    {
        // Get articles filtered by date range
        var articles = await _newsArticleRepository.FilterByDateRangeAsync(filter.StartDate, filter.EndDate);
        var articlesList = articles.ToList();

        // Category statistics
        var categories = await _categoryRepository.GetAllAsync();
        var categoryStats = categories.Select(c => new CategoryStatDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            ArticleCount = articlesList.Count(a => a.CategoryId == c.CategoryId),
            ActiveCount = articlesList.Count(a => a.CategoryId == c.CategoryId && a.NewsStatus == true),
            InactiveCount = articlesList.Count(a => a.CategoryId == c.CategoryId && a.NewsStatus == false)
        }).Where(s => s.ArticleCount > 0)
          .OrderByDescending(s => s.ArticleCount)
          .ToList();

        // Author statistics
        var accounts = await _accountRepository.GetAllAsync();
        var authorStats = accounts.Select(a => new AuthorStatDto
        {
            AccountId = a.AccountId,
            AccountName = a.AccountName,
            ArticleCount = articlesList.Count(art => art.CreatedById == a.AccountId),
            ActiveCount = articlesList.Count(art => art.CreatedById == a.AccountId && art.NewsStatus == true),
            InactiveCount = articlesList.Count(art => art.CreatedById == a.AccountId && art.NewsStatus == false)
        }).Where(s => s.ArticleCount > 0)
          .OrderByDescending(s => s.ArticleCount)
          .ToList();

        return new ReportStatisticsDto
        {
            StartDate = filter.StartDate,
            EndDate = filter.EndDate,
            TotalArticles = articlesList.Count,
            ActiveArticles = articlesList.Count(a => a.NewsStatus == true),
            InactiveArticles = articlesList.Count(a => a.NewsStatus == false),
            CategoryStats = categoryStats,
            AuthorStats = authorStats
        };
    }

    public async Task<byte[]> ExportToCsvAsync(ReportFilterDto filter)
    {
        var statistics = await GetStatisticsAsync(filter);
        var sb = new StringBuilder();

        // Add BOM for Excel to recognize UTF-8
        // Header - Summary
        sb.AppendLine("=== NEWS ARTICLE STATISTICS REPORT ===");
        sb.AppendLine();
        sb.AppendLine($"Report Period,{(statistics.StartDate?.ToString("yyyy-MM-dd") ?? "All")},{(statistics.EndDate?.ToString("yyyy-MM-dd") ?? "All")}");
        sb.AppendLine($"Total Articles,{statistics.TotalArticles}");
        sb.AppendLine($"Active Articles,{statistics.ActiveArticles}");
        sb.AppendLine($"Inactive Articles,{statistics.InactiveArticles}");
        sb.AppendLine();

        // Category Statistics
        sb.AppendLine("=== CATEGORY STATISTICS ===");
        sb.AppendLine("Category ID,Category Name,Total Articles,Active,Inactive");
        foreach (var cat in statistics.CategoryStats)
        {
            sb.AppendLine($"{cat.CategoryId},\"{cat.CategoryName}\",{cat.ArticleCount},{cat.ActiveCount},{cat.InactiveCount}");
        }
        sb.AppendLine();

        // Author Statistics
        sb.AppendLine("=== AUTHOR STATISTICS ===");
        sb.AppendLine("Account ID,Author Name,Total Articles,Active,Inactive");
        foreach (var author in statistics.AuthorStats)
        {
            sb.AppendLine($"{author.AccountId},\"{author.AccountName}\",{author.ArticleCount},{author.ActiveCount},{author.InactiveCount}");
        }

        // Return with UTF-8 BOM for Excel compatibility
        var preamble = Encoding.UTF8.GetPreamble();
        var content = Encoding.UTF8.GetBytes(sb.ToString());
        var result = new byte[preamble.Length + content.Length];
        preamble.CopyTo(result, 0);
        content.CopyTo(result, preamble.Length);

        return result;
    }
}

