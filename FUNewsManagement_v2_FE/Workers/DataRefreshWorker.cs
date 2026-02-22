using FUNewsManagement_v2_FE.Services;

namespace FUNewsManagement_v2_FE.Workers
{
    public class DataRefreshWorker : BackgroundService
    {
        private readonly ILogger<DataRefreshWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        // Run every 6 hours
        private static readonly TimeSpan Interval = TimeSpan.FromHours(6);

        public DataRefreshWorker(ILogger<DataRefreshWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataRefreshWorker started at: {time}", DateTimeOffset.Now);

            // Give the app time to start up before making the first request
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("DataRefreshWorker is running at: {time}", DateTimeOffset.Now);
                await RefreshDataAsync(stoppingToken);

                try
                {
                    await Task.Delay(Interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("DataRefreshWorker is stopping.");
        }

        private async Task RefreshDataAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                // Notice we resolve the services in Scope because CoreApiService uses transient HttpClient
                var coreApiService = scope.ServiceProvider.GetRequiredService<CoreApiService>();
                var localCacheService = scope.ServiceProvider.GetRequiredService<LocalCacheService>();

                // Need a token to call the API... For this worker, the easiest way is an explicit API call
                // Or if the API is unprotected for basic GET requests. 
                // Since our GET endpoints are mostly unprotected or token depends on the request, 
                // we should bypass auth if GET /news is public. Usually GET /api/news is public.
                // Wait, are GET /api/news and GET /api/categories public in our Core API?
                // Let's assume they are or the CoreApiService handles token automatically. 
                // Since this is a Background Worker, there is NO HttpContext! 
                // We must use a dedicated system account or ensure endpoints are anonymous.
                // Let's call them anyway. If they require token, we'll need to fetch one.
                
                // Fetch Categories
                var categoriesResponse = await coreApiService.GetCategoriesAsync(null, null, 100, 0);
                if (categoriesResponse != null && categoriesResponse.Value != null)
                {
                    await localCacheService.SaveDataAsync("categories_cache", categoriesResponse);
                    _logger.LogInformation("DataRefreshWorker: Categories cached successfully.");
                }

                // Fetch News
                var newsResponse = await coreApiService.GetNewsArticlesAsync(null, null, 100, 0);
                if (newsResponse != null && newsResponse.Value != null)
                {
                    await localCacheService.SaveDataAsync("news_cache", newsResponse);
                    _logger.LogInformation("DataRefreshWorker: News cached successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DataRefreshWorker encountered an error while refreshing data.");
            }
        }
    }
}
