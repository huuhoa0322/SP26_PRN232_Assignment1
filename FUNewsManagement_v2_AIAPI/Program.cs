
using FUNewsManagement_v2_AIAPI.Policies;
using Polly;

namespace FUNewsManagement_v2_AIAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Configure CORS to allow frontend calls
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Register Services
            builder.Services.AddSingleton<FUNewsManagement_v2_AIAPI.Services.Interfaces.ILearningCacheService, FUNewsManagement_v2_AIAPI.Services.LearningCacheService>();

            // ── Polly: circuit-breaker singleton for Gemini API ─────────────────
            // Must be created once so it correctly tracks consecutive failure counts.
            IAsyncPolicy<HttpResponseMessage>? _aiCircuitBreaker = null;

            builder.Services.AddHttpClient<FUNewsManagement_v2_AIAPI.Services.Interfaces.ITagSuggestionService, FUNewsManagement_v2_AIAPI.Services.TagSuggestionService>()
                // Retry: 3 attempts, exponential back-off + jitter, handles 5xx and 429.
                .AddPolicyHandler((sp, _) =>
                    PollyPolicies.GetAiApiRetryPolicy(
                        sp.GetRequiredService<ILogger<FUNewsManagement_v2_AIAPI.Services.TagSuggestionService>>()))
                // Circuit-breaker: lazy singleton — opens after 5 failures, stays open 60 s.
                .AddPolicyHandler((sp, _) =>
                {
                    if (_aiCircuitBreaker is not null) return _aiCircuitBreaker;
                    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Polly.GeminiApi");
                    return _aiCircuitBreaker = PollyPolicies.GetCircuitBreakerPolicy(logger);
                });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAll"); // Enable CORS
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
