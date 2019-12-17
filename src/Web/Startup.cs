using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AMTools.Web.Data.Database;
using AutoMapper;
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

            services.AddAutoMapper(GetOwnAssemblies());

            services.InjectDependencies();
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

            serviceProvider.EnsureMigrationOfContext<DatabaseContext>();
        }

        private List<Assembly> GetOwnAssemblies()
        {
            var result = new List<Assembly>();

            var mainAsssembly = Assembly.GetEntryAssembly();
            result.Add(mainAsssembly);

            foreach (AssemblyName referencedAssemblyName in mainAsssembly.GetReferencedAssemblies())
            {
                if (!referencedAssemblyName.Name.StartsWith("amtools", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                result.Add(Assembly.Load(referencedAssemblyName));
            }
            return result;
        }
    }
}
