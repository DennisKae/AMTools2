using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Shared.Core.Services.VirtualDesktops.Interfaces;
using AMTools.Web.Core.ExtensionMethods;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace AMTools.Batch
{
    public static class Program
    {
        public const string HelpTemplate = "-? | -h | --help";

        public const string DesktopOptionTemplate = "-d | -desktop | --desktop";
        public const string DesktopOptionDescription = "Nummer des Ziel-Desktops";

        public const string DontCloseOptionTemplate = "-dc | -dontclose | --dontclose";
        public const string DontCloseOptionDescription = "Lässt das Konsolenfenster offen, nachdem die Anwendung ausgeführt wurde.";

        public const string StartupCommandName = "startup";
        public const string ValidateStartupCommandName = "validate-startup";
        public const string SwitchDesktopCommandName = "switch-desktop";
        public const string CleanLogsCommandName = "clean-log";

        public static int Main(string[] args)
        {
            using (var app = new CommandLineApplication(throwOnUnexpectedArg: false) { Name = "AMTools Batch" })
            {

                app.Command(StartupCommandName, command =>
                {
                    command.Description = "Führt die StartupConfig aus.";
                    command.HelpOption(HelpTemplate);
                    CommandOption dontCloseOption = command.Option(DontCloseOptionTemplate, DontCloseOptionDescription, CommandOptionType.NoValue);

                    command.OnExecute(() => Execute(command, (IServiceProvider serviceProvider) =>
                    {
                        var service = serviceProvider.GetService<IStartupService>();
                        service.ExecuteStartupConfiguration();
                    }));
                }, throwOnUnexpectedArg: false);

                app.Command(ValidateStartupCommandName, command =>
                {
                    command.Description = "Validiert die StartupConfig.";
                    command.HelpOption(HelpTemplate);
                    CommandOption dontCloseOption = command.Option(DontCloseOptionTemplate, DontCloseOptionDescription, CommandOptionType.NoValue);

                    command.OnExecute(() => Execute(command, (IServiceProvider serviceProvider) =>
                    {
                        var service = serviceProvider.GetService<IStartupService>();
                        service.ValidateStartupConfiguration();
                    }));
                }, throwOnUnexpectedArg: false);

                app.Command(SwitchDesktopCommandName, command =>
                {
                    command.Description = "Wechselt den Desktop.";
                    command.HelpOption(HelpTemplate);

                    CommandOption desktopOption = command.Option(DesktopOptionTemplate, DesktopOptionDescription, CommandOptionType.SingleOrNoValue);
                    CommandOption dontCloseOption = command.Option(DontCloseOptionTemplate, DontCloseOptionDescription, CommandOptionType.NoValue);

                    command.OnExecute(() => Execute(command, (IServiceProvider serviceProvider) =>
                    {
                        var service = serviceProvider.GetService<IVirtualDesktopService>();
                        if (int.TryParse(desktopOption.Value(), out int parsedDesktopNummer))
                        {
                            service.Switch(parsedDesktopNummer);
                        }
                        else
                        {
                            service.SwitchRight();
                        }
                    }));

                }, throwOnUnexpectedArg: false);

                app.Command(CleanLogsCommandName, command =>
                {
                    command.Description = "Räumt die Logs auf.";
                    command.HelpOption(HelpTemplate);

                    CommandOption dontCloseOption = command.Option(DontCloseOptionTemplate, DontCloseOptionDescription, CommandOptionType.NoValue);

                    command.OnExecute(() => Execute(command, (IServiceProvider serviceProvider) =>
                    {
                        var service = serviceProvider.GetService<ILogCleanupService>();
                        service.Clean();
                    }));

                }, throwOnUnexpectedArg: false);


                app.OnExecute(() =>
                {
                    Console.Error.WriteLine("Fehler: Der Anwendung wurde kein bekannter Befehl übergeben.");
                    Console.Error.WriteLine($"Der Hilfetext der Anwendung kann mit \"{HelpTemplate}\" aufgerufen werden.");

                    return (int)ApplicationStatusCode.Error;
                });

                return app.Execute(args);
            }
        }

        private static int Execute(CommandLineApplication command, Action<IServiceProvider> method)
        {
            // How to use DI in batch applications: https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            var serviceCollection = new ServiceCollection();
            serviceCollection.EnsureMigrationOfDatabaseContext();
            serviceCollection.InjectDependencies(assemblyName, command.Name);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            ILogFactory logFactory = serviceProvider.GetService<ILogFactory>();

            ApplicationStatusCode resultStatus = ApplicationStatusCode.Success;

            var allCommandsMessageBuilder = new StringBuilder();
            try
            {
                serviceProvider.ValidateConfigurations();

                var errors = new List<string>();
                if (command.Options?.Count >= 1)
                {
                    allCommandsMessageBuilder.AppendLine("Der Anwendung wurden folgende Argumente übergeben/nicht übergeben: ");
                    allCommandsMessageBuilder.AppendLine("- CommandName: " + command.Name);
                    foreach (CommandOption commandOption in command.Options)
                    {
                        allCommandsMessageBuilder.AppendLine($"- {commandOption.LongName}: " + commandOption.Value());
                        allCommandsMessageBuilder.AppendLine("      OptionType => " + commandOption.OptionType);
                        allCommandsMessageBuilder.AppendLine("      HasValue => " + commandOption.HasValue());
                        if (commandOption.OptionType != CommandOptionType.NoValue && commandOption.OptionType != CommandOptionType.SingleOrNoValue && !commandOption.HasValue())
                        {
                            errors.Add($"Der benötigte Parameter \"{commandOption.LongName}\" wurde nicht verwendet.");
                        }
                    }
                }

                if (errors.Count >= 1)
                {
                    errors.ForEach(x => logFactory.Error(x));
                    resultStatus = ApplicationStatusCode.Error;
                }

                if (resultStatus == ApplicationStatusCode.Success)
                {
                    logFactory.ClearTempLogEntries();
                    method.Invoke(serviceProvider);
                    List<AppLog> allTempLogEntries = logFactory.GetAllTempLogEntries();
                    if (allTempLogEntries?.Count > 0)
                    {
                        if (allTempLogEntries.Any(x => x.Severity == AppLogSeverity.Exception))
                        {
                            resultStatus = ApplicationStatusCode.Exception;
                        }
                        else if (allTempLogEntries.Any(x => x.Severity == AppLogSeverity.Error))
                        {
                            resultStatus = ApplicationStatusCode.Error;
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                logFactory?.Exception(exception, "Exception auf höchster Anwendungsebene gefangen.");

                resultStatus = ApplicationStatusCode.Exception;
            }

            var endMessage = $"Der Anwendungsbefehl \"{command.Name}\" wurde mit dem Status \"{resultStatus}\" beendet.";
            if (resultStatus == ApplicationStatusCode.Error)
            {
                logFactory?.Error(endMessage);
            }
            else
            {
                logFactory?.Info(endMessage);
            }

            bool dontClose = command.Options?.Any(x => x.Description == DontCloseOptionDescription && x.HasValue()) ?? false;
            if (dontClose)
            {
                Console.WriteLine("Zum Beenden Enter Taste drücken...");
                Console.ReadLine();
            }

            return (int)resultStatus;
        }

    }
}
