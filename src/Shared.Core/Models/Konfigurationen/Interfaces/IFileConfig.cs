namespace AMTools.Shared.Core.Models.Konfigurationen.Interfaces
{
    public interface IFileConfig
    {
        string AvailabilityFile { get; set; }
        string Calloutfile { get; set; }
        string SettingsFile { get; set; }
        string SubscriberFile { get; set; }
        string CalloutfileFilename { get; }
        string DatabaseDirectory { get; set; }
        string Databasefile { get; }
    }
}