using Microsoft.IdentityModel.Tokens;
using StoreBackEnd.Api.Core.Entities;
using StoreBackEnd.Api.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StoreBackEnd.Api.Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenererAccessToken(Utilisateur utilisateur)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, utilisateur.UtilisateurID.ToString()),
                new Claim(ClaimTypes.Email, utilisateur.Email),
                new Claim(ClaimTypes.Name, utilisateur.Nom),
                new Claim(ClaimTypes.Role, utilisateur.Role.ToString())
            };

            var cle = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Cle"]!));
            var creds = new SigningCredentials(cle, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Emetteur"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:DureeMinutesAccessToken"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenererRefreshToken()
        {
            // stored server-side in the RefreshToken table so it can be revoked
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var cle = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Cle"]!));

            var parametresValidation = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Emetteur"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = cle,
                ValidateLifetime = false
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, parametresValidation, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null; 
            }

            return principal;
        }
    }
}