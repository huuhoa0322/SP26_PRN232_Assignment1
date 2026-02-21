using FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.DTOs;
using FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.Services;
using FUNewsManagement_v2_AnalyticsAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_AnalyticsAPI.DataAccess.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using System.Text;

namespace FUNewsManagement_v2_AnalyticsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // ── Database ──────────────────────────────────────────────────────
            builder.Services.AddDbContext<FunewsManagementContext>(options =>
                options.UseSqlServer(config.GetConnectionString("MyCnn")));

            // ── OData Model ───────────────────────────────────────────────────
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<DashboardItemDto>("dashboard");
            odataBuilder.EntitySet<TrendingArticleDto>("trending");

            // ── Controllers + OData ───────────────────────────────────────────
            builder.Services.AddControllers()
                .AddOData(opt => opt
                    .Select()
                    .Filter()
                    .OrderBy()
                    .Count()
                    .SetMaxTop(500)
                    .AddRouteComponents("odata", odataBuilder.GetEdmModel()));

            // ── JWT (same secret as Core API) ─────────────────────────────────
            var jwtSettings = config.GetSection("JwtSettings");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtSettings["Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            // ── App Services ──────────────────────────────────────────────────
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

            // ── CORS (allow FE origin) ────────────────────────────────────────
            builder.Services.AddCors(opt => opt.AddPolicy("AllowFE", policy =>
                policy.WithOrigins(
                    config["AllowedOrigins"] ?? "https://localhost:7000",
                    "https://localhost:7001",
                    "http://localhost:5000",
                    "http://localhost:5001")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));

            // ── Swagger ───────────────────────────────────────────────────────
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "FUNews Analytics API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập JWT token (không cần prefix 'Bearer')"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
                        }, []
                    }
                });
            });

            var app = builder.Build();

            // ── Pipeline ──────────────────────────────────────────────────────
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFE");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
