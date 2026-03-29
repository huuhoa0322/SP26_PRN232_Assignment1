using FUNewsManagement_v2_FE.Policies;
using Polly;

namespace FUNewsManagement_v2_FE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(); // For API proxy controllers
            builder.Services.AddRazorPages();
            
            // Add Session support
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            
            // Add HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // ── Polly: stateful circuit-breakers (singleton per named client) ──────
            // Circuit-breakers must be created once and reused so they correctly track
            // consecutive failure counts across requests.
            IAsyncPolicy<HttpResponseMessage>? _coreCircuitBreaker = null;
            IAsyncPolicy<HttpResponseMessage>? _analyticsCircuitBreaker = null;

            // Add HttpClient for CoreApiService with Polly retry + circuit-breaker
            var coreApiBaseUrl = builder.Configuration["CoreApiSettings:BaseUrl"] ?? "https://localhost:7001";
            builder.Services.AddHttpClient<FUNewsManagement_v2_FE.Services.CoreApiService>(client =>
            {
                client.BaseAddress = new Uri(coreApiBaseUrl);
            })
            // Retry: only for write operations (POST/PUT/DELETE/PATCH).
            // GET requests must NOT retry — fail fast so the cache fallback
            // shows cached data immediately instead of waiting ~14 s.
            .AddPolicyHandler((sp, request) =>
            {
                if (request.Method == HttpMethod.Get)
                    return Policy.NoOpAsync<HttpResponseMessage>();
                return PollyPolicies.GetRetryPolicy(
                    sp.GetRequiredService<ILogger<FUNewsManagement_v2_FE.Services.CoreApiService>>());
            })
            // Circuit-breaker: lazy singleton — opens after 5 failures, stays open 30 s.
            .AddPolicyHandler((sp, _) =>
            {
                if (_coreCircuitBreaker is not null) return _coreCircuitBreaker;
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Polly.CoreApi");
                return _coreCircuitBreaker = PollyPolicies.GetCircuitBreakerPolicy(logger);
            });

            // Add HttpClient for AnalyticsApiService with Polly retry + circuit-breaker
            var analyticsApiBaseUrl = builder.Configuration["AnalyticsApiSettings:BaseUrl"] ?? "http://localhost:5142";
            builder.Services.AddHttpClient<FUNewsManagement_v2_FE.Services.AnalyticsApiService>(client =>
            {
                client.BaseAddress = new Uri(analyticsApiBaseUrl);
            })
            // Retry: only for write operations — GET fails fast to cache.
            .AddPolicyHandler((sp, request) =>
            {
                if (request.Method == HttpMethod.Get)
                    return Policy.NoOpAsync<HttpResponseMessage>();
                return PollyPolicies.GetRetryPolicy(
                    sp.GetRequiredService<ILogger<FUNewsManagement_v2_FE.Services.AnalyticsApiService>>());
            })
            // Circuit-breaker: separate singleton so Analytics faults don't affect Core API.
            .AddPolicyHandler((sp, _) =>
            {
                if (_analyticsCircuitBreaker is not null) return _analyticsCircuitBreaker;
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Polly.AnalyticsApi");
                return _analyticsCircuitBreaker = PollyPolicies.GetCircuitBreakerPolicy(logger);
            });

            // Add Cache Service and Background Worker for Offline Mode
            builder.Services.AddSingleton<FUNewsManagement_v2_FE.Services.LocalCacheService>();
            builder.Services.AddHostedService<FUNewsManagement_v2_FE.Workers.DataRefreshWorker>();

            var app = builder.Build();

            // Custom Middleware for Offline Fallback 
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (HttpRequestException ex)
                {
                    // If the backend refuses connection or times out, redirect to Offline page.
                    // Make sure it's not an API route or an ajax request first, otherwise it should return JSON
                    if (context.Request.Path.StartsWithSegments("/api") || context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        context.Response.StatusCode = 503;
                        await context.Response.WriteAsync("Service Unavailable - Offline Mode");
                    }
                    else
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.Response.Redirect("/Offline");
                        }
                    }
                }
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            
            // Use Session before Authorization
            app.UseSession();

            app.UseAuthorization();

            app.MapControllers(); // Map API proxy controllers
            app.MapRazorPages();

            app.Run();
        }
    }
}
 