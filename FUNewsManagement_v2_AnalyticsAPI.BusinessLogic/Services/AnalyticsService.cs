using ClosedXML.Excel;
using FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.DTOs;
using FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_AnalyticsAPI.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly FunewsManagementContext _db;

        public AnalyticsService(FunewsManagementContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns a flat IQueryable for OData filtering.
        /// Each row = one article with its category and author info.
        /// OData $filter / $orderby / $top are applied by the framework.
        /// </summary>
        public IQueryable<DashboardItemDto> GetDashboardQuery()
        {
            return _db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Select(n => new DashboardItemDto
                {
                    NewsArticleId = n.NewsArticleId,
                    NewsTitle = n.NewsTitle,
                    CreatedDate = n.CreatedDate,
                    NewsStatus = n.NewsStatus,
                    CategoryId = n.CategoryId,
                    CategoryName = n.Category != null ? n.Category.CategoryName : null,
                    CreatedById = n.CreatedById,
                    AuthorName = n.CreatedBy != null ? n.CreatedBy.AccountName : null,
                });
        }

        /// <summary>
        /// Returns active articles ordered by recency + tag count (hotness).
        /// OData $top / $filter are applied at HTTP level.
        /// </summary>
        public IQueryable<TrendingArticleDto> GetTrendingQuery()
        {
            return _db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .Where(n => n.NewsStatus == true)
                .OrderByDescending(n => n.Tags.Count)
                .ThenByDescending(n => n.CreatedDate)
                .Select(n => new TrendingArticleDto
                {
                    NewsArticleId = n.NewsArticleId,
                    NewsTitle = n.NewsTitle,
                    Headline = n.Headline,
                    CreatedDate = n.CreatedDate,
                    CategoryId = n.CategoryId,
                    CategoryName = n.Category != null ? n.Category.CategoryName : null,
                    ImageUrl = n.ImageUrl,
                    TagCount = n.Tags.Count,
                });
        }

        /// <summary>
        /// Exports an Excel report with 3 sheets: summary, by category, by author.
        /// Optional date range filter applied in-memory.
        /// </summary>
        public async Task<byte[]> ExportExcelAsync(DateTime? startDate, DateTime? endDate)
        {
            // Fetch data — apply date filter here since OData not used on export
            var query = _db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(n => n.CreatedDate <= endDate.Value.AddDays(1));

            var articles = await query.ToListAsync();

            using var workbook = new XLWorkbook();

            // ── Sheet 1: Summary ──────────────────────────────────────────────
            var ws1 = workbook.Worksheets.Add("Tổng quan");
            ws1.Cell(1, 1).Value = "Tổng bài viết";
            ws1.Cell(1, 2).Value = articles.Count;
            ws1.Cell(2, 1).Value = "Bài Active";
            ws1.Cell(2, 2).Value = articles.Count(a => a.NewsStatus == true);
            ws1.Cell(3, 1).Value = "Bài Inactive";
            ws1.Cell(3, 2).Value = articles.Count(a => a.NewsStatus != true);
            ws1.Cell(4, 1).Value = "Từ ngày";
            ws1.Cell(4, 2).Value = startDate.HasValue ? startDate.Value.ToString("dd/MM/yyyy") : "Tất cả";
            ws1.Cell(5, 1).Value = "Đến ngày";
            ws1.Cell(5, 2).Value = endDate.HasValue ? endDate.Value.ToString("dd/MM/yyyy") : "Tất cả";
            ws1.Column(1).Width = 20;
            ws1.Column(2).Width = 20;
            StyleHeaderRow(ws1, 1, 2);

            // ── Sheet 2: By Category ──────────────────────────────────────────
            var ws2 = workbook.Worksheets.Add("Theo danh mục");
            ws2.Cell(1, 1).Value = "Danh mục";
            ws2.Cell(1, 2).Value = "Tổng bài";
            ws2.Cell(1, 3).Value = "Active";
            ws2.Cell(1, 4).Value = "Inactive";
            StyleHeaderRow(ws2, 1, 4);

            var byCategory = articles
                .GroupBy(a => a.Category?.CategoryName ?? "Không có")
                .OrderByDescending(g => g.Count())
                .ToList();

            int row2 = 2;
            foreach (var g in byCategory)
            {
                ws2.Cell(row2, 1).Value = g.Key;
                ws2.Cell(row2, 2).Value = g.Count();
                ws2.Cell(row2, 3).Value = g.Count(a => a.NewsStatus == true);
                ws2.Cell(row2, 4).Value = g.Count(a => a.NewsStatus != true);
                row2++;
            }
            ws2.Columns().AdjustToContents();

            // ── Sheet 3: By Author ────────────────────────────────────────────
            var ws3 = workbook.Worksheets.Add("Theo tác giả");
            ws3.Cell(1, 1).Value = "Tác giả";
            ws3.Cell(1, 2).Value = "Tổng bài";
            ws3.Cell(1, 3).Value = "Active";
            ws3.Cell(1, 4).Value = "Inactive";
            StyleHeaderRow(ws3, 1, 4);

            var byAuthor = articles
                .GroupBy(a => a.CreatedBy?.AccountName ?? "Ẩn danh")
                .OrderByDescending(g => g.Count())
                .ToList();

            int row3 = 2;
            foreach (var g in byAuthor)
            {
                ws3.Cell(row3, 1).Value = g.Key;
                ws3.Cell(row3, 2).Value = g.Count();
                ws3.Cell(row3, 3).Value = g.Count(a => a.NewsStatus == true);
                ws3.Cell(row3, 4).Value = g.Count(a => a.NewsStatus != true);
                row3++;
            }
            ws3.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static void StyleHeaderRow(IXLWorksheet ws, int row, int lastCol)
        {
            for (int c = 1; c <= lastCol; c++)
            {
                ws.Cell(row, c).Style.Font.Bold = true;
                ws.Cell(row, c).Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                ws.Cell(row, c).Style.Font.FontColor = XLColor.White;
            }
        }
    }
}
