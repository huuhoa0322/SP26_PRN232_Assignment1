
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Helpers;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Mappings;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.AuditLog;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;

namespace FUNewsManagement_v2_CoreAPI
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure DbContext
            builder.Services.AddDbContext<FunewsManagementContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            // Register Repositories
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();

            // Register Services
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<INewsService, NewsService>();
            builder.Services.AddScoped<JwtHelper>();

            // Configure AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Configure FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<AutoMapperProfile>();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
                    };
                });

            // Build OData EDM Model
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<AccountDto>("Accounts");
            modelBuilder.EntityType<AccountDto>().HasKey(a => a.AccountId);

            modelBuilder.EntitySet<AuditLogDto>("AuditLogs");
            modelBuilder.EntityType<AuditLogDto>().HasKey(a => a.LogId);
            modelBuilder.EntitySet<CategoryDto>("Categories");
            modelBuilder.EntityType<CategoryDto>().HasKey(c => c.CategoryId);

            modelBuilder.EntitySet<TagDto>("Tags");
            modelBuilder.EntityType<TagDto>().HasKey(t => t.TagId);

            modelBuilder.EntitySet<NewsArticleDto>("NewsArticles");
            modelBuilder.EntityType<NewsArticleDto>().HasKey(n => n.NewsArticleId);
            
            // Configure CORS for Frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5000", 
                            "https://localhost:5001",
                            "http://localhost:5212",      // Add FE port
                            "http://localhost:25949")     // Add IIS Express port
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Add Controllers with OData support
            builder.Services.AddControllers()
                .AddOData(options => options
                    .Select()           // Enable $select
                    .Filter()           // Enable $filter
                    .OrderBy()          // Enable $orderby
                    .Expand()           // Enable $expand
                    .Count()            // Enable $count
                    .SetMaxTop(100)     // Limit $top to 100
                    .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

            builder.Services.AddEndpointsApiExplorer();
            
            // Configure Swagger with JWT
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "FUNewsManagement Core API",
                    Version = "v1",
                    Description = "Core API with JWT Authentication, OData, and Admin Features"
                });

                // Define JWT Bearer security scheme
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
                });

                // Require JWT for all endpoints
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
                
                // Ignore OData controllers to avoid conflicts
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    // Exclude OData controllers (they start with /odata)
                    if (apiDesc.RelativePath?.StartsWith("odata/") == true)
                        return false;
                    return true;
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            // Enable CORS
            app.UseCors("AllowFrontend");

            app.UseStaticFiles(); // Serve uploaded images

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
 