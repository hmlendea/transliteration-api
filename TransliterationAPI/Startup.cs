using System;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using TransliterationAPI.Configuration;

namespace TransliterationAPI
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;
        public static IServiceProvider ServiceProvider { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddConfigurations(Configuration);
            services.AddCustomServices();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TransliterationAPI", Version = "v1" });
            });

            ServiceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TransliterationAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            CacheSettings cacheSettings = ServiceProvider.GetService<CacheSettings>();
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
