using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniStore.Service.Utilities
{
    public static class AuthJWT
    {
        public static readonly int ACCESS_TOKEN_EXPIRED = 2;
        public static readonly int REFRESH_TOKEN_EXPIRED = 2 * 24;

        public static string GenerateToken(this UserInfo user, int expiredDate)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(configuration["JwtAuth:Key"]);
            var claimList = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserId.ToString()),
                //new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Typ, user.Status.ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration["JwtAuth:Issuer"],
                Subject = new ClaimsIdentity(claimList),
                Audience = configuration["JwtAuth:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(expiredDate),
                SigningCredentials = new SigningCredentials(
                                        new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
