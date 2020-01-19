using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AMTools.Core.Services.Logging;
using AMTools.Shared.Core.Models;
using AMTools.Shared.Core.Services.Interfaces;
using AMTools.Web.Core.ExtensionMethods;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.VirtualDesktops.Interfaces;
using AMTools.Web.Data.Database;
using AMTools.Web.Data.Database.Models;
using AMTools.Web.Data.JsonStore.Models;
using AMTools.Web.Data.JsonStore.Repositories;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace AMTools.Batch
{
    public static class Program
    {
        public const string HelpTemplate = "--help";

        public const string DesktopOptionTemplate = "--desktop";
        public const string DesktopOptionDescription = "Nummer des Ziel-Desktops";

        public const string DontCloseOptionTemplate = "--dontclose";
        public const string DontCloseOptionDescription = "Lässt das Konsolenfenster offen, nachdem die Anwendung ausgeführt wurde.";

        public const string InputDirectoryOptionTemplate = "--input-directory";
        public const string InputDirectoryOptionDescription = "Pfad zum Ordner, aus dem heraus importiert werden soll.";

        public const string PatternOptionTemplate = "--pattern";
        public const string PatternOptionDescription = "Muster, z.B. für die Identifizierung von Dateinamen";

        public const string StartupCommandName = "startup";
        public const string ValidateStartupCommandName = "validate-startup";
        public const string SwitchDesktopCommandName = "switch-desktop";
        public const string CleanLogsCommandName = "clean-log";
        public const string RebootCommandName = "reboot";
        public const string CalloutImportCommandName = "callout-import";

        public static int Main(string[] args)
        {
            using (var app = new CommandLineApplication(throwOnUnexpectedArg: false) { Name = "AMTools Batch" })
            {
                app.HelpOption(HelpTemplate);

                app.Command(StartupCommandName, command =>
                {
                    command.Description = "Führt die StartKonfiguration aus.";
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
                    command.Description = "Validiert die StartKonfiguration.";
                    command.HelpOption(HelpTemplate);
                    CommandOption dontCloseOption = command.Option(DontCloseOptionTemplate, DontCloseOptionDescription, CommandOptionType.NoValue);

                    command.OnExecute(() => Execute(command, (IServiceProvider serviceProvider) =>
                    {
                        var service = serviceProvider.GetService<IStartupService>();
                        service.ValidateStartKonfiguration();
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
                            service.Switch(parsedDesktopNummer - 1);
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

                app.Command(RebootCommandName, command =>
                {
                    command.Description = "Führt einen Neustart durch.";
                    command.HelpOption(HelpTemplate);

                    command.OnExecute(() => Execute(command, (IServiceProvider serviceProvider) =>
                    {
                        var logService = serviceProvider.GetService<ILogService>();
                        logService?.Info("Ein Neustart des Computers wird eingeleitet.");

                        var terminalService = serviceProvider.GetService<ITerminalService>();
                        terminalService.Execute("shutdown /r");
                    }));

                }, throwOnUnexpectedArg: false);

                app.Command(CalloutImportCommandName, command =>
                {
                    command.Description = "Importiert existierende Callout-Dateien.";
                    command.HelpOption(HelpTemplate);

                    CommandOption dontCloseOption = command.Option(DontCloseOptionTemplate, DontCloseOptionDescription, CommandOptionType.NoValue);
                    CommandOption inputDirectoryOption = command.Option(InputDirectoryOptionTemplate, InputDirectoryOptionDescription, CommandOptionType.SingleValue);
                    CommandOption patternOption = command.Option(PatternOptionTemplate, PatternOptionDescription, CommandOptionType.SingleValue);

                    command.OnExecute(() => Execute(command, (IServiceProvider serviceProvider) =>
                    {
                        var service = serviceProvider.GetService<IBatchCalloutImportService>();
                        service.Import(inputDirectoryOption.Value(), patternOption.Value());
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
                    allCommandsMessageBuilder.AppendLine("Die Anwendung wird mit folgendem Befehl und den folgenden Argumenten ausgeführt: ");
                    allCommandsMessageBuilder.AppendLine("- Befehl: " + command.Name);
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
                logFactory.Info(allCommandsMessageBuilder.ToString());

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
