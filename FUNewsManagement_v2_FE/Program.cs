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
            
            // Add HttpClient for CoreApiService
            var coreApiBaseUrl = builder.Configuration["CoreApiSettings:BaseUrl"] ?? "https://localhost:7001";
            builder.Services.AddHttpClient<FUNewsManagement_v2_FE.Services.CoreApiService>(client =>
            {
                client.BaseAddress = new Uri(coreApiBaseUrl);
            });

            var app = builder.Build();

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
 