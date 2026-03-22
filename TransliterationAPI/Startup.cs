using System;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NuciAPI.Middleware;
using TransliterationAPI.Configuration;

namespace TransliterationAPI
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddConfigurations(Configuration);
            services.AddCustomServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            CacheSettings cacheSettings)
        {
            app.UseNuciApiExceptionHandling();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (!File.Exists(cacheSettings.StoreLocation))
            {
                CreateCacheStore(cacheSettings.StoreLocation);
            }
        }

        public static void CreateCacheStore(string storeLocation)
        {
            string storeDirectoryLocation = Path.GetDirectoryName(storeLocation);

            if (!string.IsNullOrWhiteSpace(storeDirectoryLocation))
            {
                Directory.CreateDirectory(storeDirectoryLocation);
            }

            File.WriteAllText(storeLocation, "[]");
        }
    }
}
