
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
            builder.Services.AddHttpClient<FUNewsManagement_v2_AIAPI.Services.Interfaces.ITagSuggestionService, FUNewsManagement_v2_AIAPI.Services.TagSuggestionService>();

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
