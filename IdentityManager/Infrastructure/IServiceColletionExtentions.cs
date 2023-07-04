using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityManager.Models;

namespace IdentityManager.Infrastructure
{
    public static class IServiceColletionExtentions
    {
        public static void AddJwtTokenValidation(this IServiceCollection services)
        {
            services.AddAuthentication(options=>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(options=> 
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = TokenService.Issuer,
                    ValidAudience = TokenService.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenService.Key))
                };
            });
        }

        public static void AddJwtService(this IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();
        }
    }
}
