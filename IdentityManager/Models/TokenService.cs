using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityManager.Models
{
    public class TokenService : ITokenService
    {
        public static readonly string Key = "$NotifyerProSuperSecretKey$";
        public static readonly string Issuer = "Denis_Surmanidze";
        public static readonly string Audience = "App_Users";

        public string GenerateToken(RequestUser user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            ClaimsIdentity identity = 
                new ClaimsIdentity
                (claims, JwtBearerDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

            var credentials = new SigningCredentials
                (new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key)), SecurityAlgorithms.HmacSha256);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateJwtSecurityToken
                (
                    subject:identity,
                    issuer: Issuer,
                    audience: Audience,
                    signingCredentials: credentials,
                    expires: DateTime.Now.AddMinutes(20)
                );

            return handler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(token, new TokenValidationParameters() 
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key))
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
