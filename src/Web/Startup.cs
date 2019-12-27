using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AMTools.Web.BackgroundServices;
using AMTools.Web.Core.ExtensionMethods;
using AMTools.Web.Data.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AMTools.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    // Necessary to disable lowerCamelCasing and use PascalCasing for regular api responses
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;

                    options.JsonSerializerOptions.WriteIndented = true;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = Assembly.GetExecutingAssembly().GetName().Name + " API",
                    Version = "v1"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.EnsureMigrationOfDatabaseContext();
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            services.InjectDependencies(assemblyName);
            services.InjectBackgroundServices();

            services.AddMemoryCache(action => { action.CompactionPercentage = .25; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", Assembly.GetExecutingAssembly().GetName().Name + " API V1");
            });

            serviceProvider.ValidateConfigurations();
        }
    }
}
