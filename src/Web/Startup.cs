using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Repositories;
using AMTools.Shared.Core.Repositories.Interfaces;
using AMTools.Shared.Core.Services;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Shared.Core.Services.Logging;
using AMTools.Web.BackgroundServices;
using AMTools.Web.Core.Services.DataSynchronization;
using AMTools.Web.Core.Services.DataSynchronization.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Files;
using AMTools.Web.Data.Files.Repositories;
using AMTools.Web.Data.Files.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

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

            InjectDependencies(services);
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

        private void InjectDependencies(IServiceCollection services)
        {
            // Singleton means only a single instance will ever be created. That instance is shared between all components that require it. The same instance is thus used always.
            // Scoped means an instance is created once per scope. A scope is created on every request to the application, thus any components registered as Scoped will be created once per request.
            // Transient components are created every time they are requested and are never shared.

            ILogFactory logFactory = GetLogFactory();

            try
            {
                services.AddDbContext<DatabaseContext>();
                services.AddSingleton<IConfigurationFileRepository, ConfigurationFileRepository>();

                services.AddSingleton(GetMapper());
                services.AddSingleton(logFactory);
                services.AddSingleton<ILogService>(serviceProvider =>
                {
                    ILogFactory logFactory = serviceProvider.GetService<ILogFactory>();
                    return logFactory;
                });

                // FileRepositories
                services.AddSingleton<IAvailabilityFileRepository, AvailabilityFileRepository>();
                services.AddSingleton<ICalloutFileRepository, CalloutFileRepository>();
                services.AddSingleton<ISettingsFileRepository, SettingsFileRepository>();
                services.AddSingleton<ISubscriberFileRepository, SubscriberFileRepository>();

                // SyncServices
                services.AddSingleton<IAlertSyncService, AlertSyncService>();
                services.AddSingleton<IAvailabilityStatusSyncService, AvailabilityStatusSyncService>();
                services.AddSingleton<ISettingsSyncService, SettingsSyncService>();
                services.AddSingleton<ISubscriberSyncService, SubscriberSyncService>();
                services.AddSingleton<IUserResponseSyncService, UserResponseSyncService>();

                // BackgroundServices
                services.AddHostedService<SettingsBackgroundService>();
                services.AddHostedService<AvailabilityStatusBackgroundService>();
                services.AddHostedService<SubscriberBackgroundService>();
                services.AddHostedService<CalloutBackgroundService>();

                // Andere Services
                services.AddSingleton<ITerminalService, TerminalService>();
                services.AddSingleton<IVirtualDesktopVersionService, VirtualDesktopVersionService>();
                services.AddSingleton<IVirtualDesktopWrapperService, VirtualDesktopWrapperService>();
                services.AddSingleton<IVirtualDesktopService, VirtualDesktopService>();
            }
            catch (Exception exception)
            {
                logFactory.Exception(exception, "Exception bei der Dependency Injection.");
            }
        }

        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FileImportMapProfile>();
                cfg.AddProfile<DatabaseMapProfile>();
            });

            return new Mapper(config);
        }

        private ILogFactory GetLogFactory()
        {
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var consoleLogService = new ConsoleLogService(assemblyName, null);
            var dbLogService = new DbLogService(assemblyName, null, consoleLogService);

            var result = new LogFactory(consoleLogService, assemblyName, null);
            result.LoggingServices.Add(dbLogService);
            return result;
        }
    }
}
