
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Helpers;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Mappings;
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

            // Register Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
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
            modelBuilder.EntitySet<SystemAccount>("Accounts");
            modelBuilder.EntitySet<AuditLog>("AuditLogs");

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
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
