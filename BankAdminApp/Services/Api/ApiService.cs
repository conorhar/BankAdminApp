using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BankAdminApp.Services.API
{
    public class ApiService : IApiService
    {

        private string SecretKey = "312121312<znxmb4";
        private string Issuer = "bankadminapp";

        public string GenerateJSONWebToken(string id)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);



            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("roles","Customer" )
            };

            var token = new JwtSecurityToken(Issuer,
                Issuer,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}