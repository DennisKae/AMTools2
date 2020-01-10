using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AMTools.Web.BackgroundServices;
using AMTools.Web.Core.ExtensionMethods;
using AMTools.Web.Data.Database;
using AMTools.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

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

            // SignalR
            services
                .AddSignalR(config =>
                {
                    config.ClientTimeoutInterval = TimeSpan.FromHours(24);
                    config.EnableDetailedErrors = true;
                })
                .AddJsonProtocol(options =>
                {
                    // Necessary to disable lowerCamelCasing and use PascalCasing for SignalR responses
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                    options.PayloadSerializerOptions.WriteIndented = true;
                });

            // Static files
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            // Swagger
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

            // Injections
            services.EnsureMigrationOfDatabaseContext();
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            services.InjectDependencies(assemblyName);
            services.InjectBackgroundServices();

            // MemoryCache injection
            services.AddMemoryCache(action => { action.CompactionPercentage = .25; });

            // TODO Prio 96: HealthChecks?
            // services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

#if DEBUG
            // CORS
            // Achtung: Die Reihenfolge (vor UseEndpoints) ist wichtig!
            // https://docs.microsoft.com/de-de/aspnet/core/signalr/security?view=aspnetcore-3.1
            //app.UseCorsMiddleware();
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    //.WithHeaders("Content-Type", "X-CSRF-Token", "X-Requested-With", "Accept", "Accept-Version", "Content-Length", "Content-MD5", "Date", "X-Api-Version", "X-File-Name")
                    .WithMethods("POST", "GET", "PUT", "PATCH", "DELETE", "OPTIONS")
                    .AllowCredentials();
            });
#endif

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AvailabilityHub>("/availability");
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", Assembly.GetExecutingAssembly().GetName().Name + " API V1");
            });

            serviceProvider.ValidateConfigurations();
            hostApplicationLifetime.LogAppStatusChanges(serviceProvider);

            app.UseSpaStaticFiles();

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "Frontend";

                //if (env.IsDevelopment())
                //{
                //    spa.UseAngularCliServer(npmScript: "start");
                //}
            });
        }
    }
}
