using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using IdentityManager.Models;
using IdentityManager.Infrastructure;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;

namespace OcelotApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();
            services.AddMemoryCache();
            services.AddJwtTokenValidation();
            services.AddOcelot(Configuration);
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
           {
               string token = string.Empty;

               cache.TryGetValue("token", out token);

               if (!string.IsNullOrEmpty(token))
               {
                   context.Request.Headers.Add("Authorization", $"Bearer {cache.Get("token").ToString()}");
                   context.Response.Headers.Add("Authorization", $"Bearer {cache.Get("token").ToString()}");
               }

               await next.Invoke();
           });
           
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            app.UseOcelot().Wait();
        }
    }
}
