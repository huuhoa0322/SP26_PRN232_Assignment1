using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Helpers
{
    /// <summary>
    /// Helper class để generate và validate JWT tokens
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generate access token cho account
        /// </summary>
        public string GenerateAccessToken(SystemAccount account) 
        {
            // Determine role name: Staff=1, Lecturer=2, Admin from appsettings email
            string roleName;
            var adminEmail = _configuration["AdminAccount:Email"];
            if (!string.IsNullOrEmpty(adminEmail) && account.AccountEmail.Equals(adminEmail, StringComparison.OrdinalIgnoreCase))
            {
                roleName = "Admin";
            }
            else if (account.AccountRole == 1)
            {
                roleName = "Staff";
            }
            else if (account.AccountRole == 2)
            {
                roleName = "Lecturer";
            }
            else
            {
                roleName = "Unknown";
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Email, account.AccountEmail!),
                new Claim(ClaimTypes.Name, account.AccountName!),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generate refresh token (random string)
        /// </summary>
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Validate JWT token và return ClaimsPrincipal
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
