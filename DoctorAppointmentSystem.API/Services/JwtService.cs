using DoctorAppointmentSystem.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoctorAppointmentSystem.API.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(Doctor doctor)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, doctor.Id),
                new Claim(ClaimTypes.Name, doctor.FullName),
                new Claim(ClaimTypes.Role, "Doctor")
            };

            return GenerateToken(claims);
        }

        public string GenerateJwtToken(Pharmacy pharmacy)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, pharmacy.Id),
                new Claim(ClaimTypes.Name, pharmacy.Name),
                new Claim(ClaimTypes.Role, "Pharmacy") 
            };

            return GenerateToken(claims);
        }

        private string GenerateToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = key,
                ValidateLifetime = false 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            return principal;
        }

        public string GetUserRole(string token)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var role = principal.FindFirstValue(ClaimTypes.Role);
            return role;
        }
    }
}
