using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsArticleRepository _newsRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IAuditLogService _auditLogService;
        private readonly INotificationService _notificationService;
        private readonly IAccountRepository _accountRepository;

        public NewsService(
            INewsArticleRepository newsRepository, 
            ITagRepository tagRepository,
            IMapper mapper,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            IAuditLogService auditLogService,
            INotificationService notificationService,
            IAccountRepository accountRepository)
        {
            _newsRepository = newsRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
            _environment = environment;
            _configuration = configuration;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<NewsArticleDto>> GetAllAsync()
        {
            // Note: For OData list, better to return IQueryable or rely on repository returning entities
            // But here we return DTOs. OData controller will handle [EnableQuery] on this list.
            // Ideally OData should work on IQueryable<Entity> or IQueryable<Dto>.
            // Since our repository returns List for generic getAll, we might fetch all into memory then map.
            // For valid OData pagination database-side, we should expose IQueryable in Repository.
            // Current GenericRepository returns IEnumerable from ToListAsync.
            // Implementation of GenericRepository.GetAllAsync loads all data.
            // This is okay for small datasets but not for News.
            // However, sticking to the existing pattern for now.
            var articles = await _newsRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<NewsArticleDto>>(articles);
        }

        public async Task<NewsArticleDto?> GetByIdAsync(string id)
        {
            var article = await _newsRepository.GetByIdWithDetailsAsync(id);
            return article == null ? null : _mapper.Map<NewsArticleDto>(article);
        }

        private async Task<string?> UploadImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path or full URL. Usually relative path stored in DB.
            // "uploads/filename.jpg"
            return "uploads/" + uniqueFileName;
        }

        public async Task<NewsArticleDto> CreateAsync(CreateNewsArticleRequest request, short userId)
        {
            var article = _mapper.Map<NewsArticle>(request);
            
            // Generate sequential numeric ID (max existing + 1)
            article.NewsArticleId = await _newsRepository.GetNextIdAsync();
            
            article.CreatedById = userId;
            article.CreatedDate = DateTime.Now;
            article.ModifiedDate = DateTime.Now;
            
            // Handle Image Upload
            if (request.ImageFile != null)
            {
                article.ImageUrl = await UploadImageAsync(request.ImageFile);
            }
            
            // Handle Tags
            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    var tag = await _tagRepository.GetByIdAsync(tagId);
                    if (tag != null)
                    {
                        article.Tags.Add(tag);
                    }
                }
            }

            await _newsRepository.AddAsync(article);
            var resultDto = _mapper.Map<NewsArticleDto>(article);

            await _auditLogService.LogActionAsync("Create", "NewsArticle", article.NewsArticleId, null, resultDto);

            // Fetch createdByName and trigger Realtime Notification
            var account = await _accountRepository.GetByIdAsync(userId);
            string createdByName = account?.AccountName ?? "Staff";
            await _notificationService.SendNewsCreatedNotificationAsync(resultDto.NewsTitle, createdByName, resultDto.CreatedDate ?? DateTime.Now);

            return resultDto;
        }

        public async Task<NewsArticleDto?> UpdateAsync(string id, UpdateNewsArticleRequest request, short userId)
        {
            var article = await _newsRepository.GetByIdWithDetailsAsync(id);
            if (article == null) return null;

            var oldData = _mapper.Map<NewsArticleDto>(article);

            _mapper.Map(request, article);
            article.UpdatedById = userId;
            article.ModifiedDate = DateTime.Now;

            // Handle Image Upload
            if (request.ImageFile != null)
            {
                // Optionally delete old image
                 article.ImageUrl = await UploadImageAsync(request.ImageFile);
            }

            // Handle Tags
            if (request.TagIds != null)
            {
                article.Tags.Clear();
                foreach (var tagId in request.TagIds)
                {
                    var tag = await _tagRepository.GetByIdAsync(tagId);
                    if (tag != null)
                    {
                        article.Tags.Add(tag);
                    }
                }
            }

            await _newsRepository.UpdateAsync(article);
            var newData = _mapper.Map<NewsArticleDto>(article);

            await _auditLogService.LogActionAsync("Update", "NewsArticle", id, oldData, newData);

            return newData;
        }

        public async Task<bool> DeleteAsync(string id, short userId)
        {
            var article = await _newsRepository.GetByIdWithDetailsAsync(id);
            if (article == null) return false;

            // Enforce author-only delete
            if (article.CreatedById != userId)
                throw new UnauthorizedAccessException("Bạn chỉ có thể xóa bài viết do chính mình tạo");

            var oldData = _mapper.Map<NewsArticleDto>(article);

            // Clear Tags to avoid FK constraint on NewsTag table
            article.Tags.Clear();
            await _newsRepository.UpdateAsync(article);

            await _newsRepository.DeleteAsync(article);
            
            await _auditLogService.LogActionAsync("Delete", "NewsArticle", id, oldData, null);

            return true;
        }

        public async Task<NewsArticleDto?> DuplicateAsync(string id, short userId)
        {
            var original = await _newsRepository.GetByIdWithDetailsAsync(id);
            if (original == null) return null;

            var copy = new NewsArticle
            {
                NewsArticleId = await _newsRepository.GetNextIdAsync(),
                NewsTitle = original.NewsTitle + " (Copy)",
                Headline = original.Headline,
                NewsContent = original.NewsContent,
                NewsSource = original.NewsSource,
                CategoryId = original.CategoryId,
                NewsStatus = false, // Default to inactive for copy
                CreatedById = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ImageUrl = original.ImageUrl, 
                // Copy Tags
                Tags = original.Tags.ToList() 
            };

            await _newsRepository.AddAsync(copy);
            var resultDto = _mapper.Map<NewsArticleDto>(copy);
            
            await _auditLogService.LogActionAsync("Create (Duplicate)", "NewsArticle", copy.NewsArticleId, null, resultDto);

            return resultDto;
        }

        public async Task<IEnumerable<NewsArticleDto>> GetRecommendAsync(string articleId, int count = 3)
        {
            var original = await _newsRepository.GetByIdWithDetailsAsync(articleId);
            if (original == null || original.CategoryId == null) return new List<NewsArticleDto>();

            var all = await _newsRepository.GetAllAsync();
            var related = all.Where(n => n.CategoryId == original.CategoryId && 
                                         n.NewsArticleId != articleId && 
                                         n.NewsStatus == true)
                             .OrderByDescending(n => n.CreatedDate)
                             .Take(count);
                             
            return _mapper.Map<IEnumerable<NewsArticleDto>>(related);
        }
    }
}
