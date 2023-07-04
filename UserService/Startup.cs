using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityManager.Models;
using IdentityManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using Microsoft.Extensions.Caching.Memory;
using UserService.ProtoServices;

namespace UserService
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

            services.AddControllersWithViews();
            services.AddJwtTokenValidation();
            services.AddControllers();
            services.AddSession();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:AppConnection"]);
            });
            services.AddMemoryCache();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddGrpc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<MessangerService>();
                endpoints.MapControllers();
            });
        }
    }
}
