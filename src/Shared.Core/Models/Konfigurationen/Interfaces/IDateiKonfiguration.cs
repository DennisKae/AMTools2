namespace AMTools.Shared.Core.Models.Konfigurationen.Interfaces
{
    public interface IDateiKonfiguration
    {
        string AvailabilityDatei { get; set; }
        string CalloutDatei { get; set; }
        string SettingsDatei { get; set; }
        string SubscriberDatei { get; set; }
        string Datenbankordner { get; set; }
        string Datenbankpfad { get; }
    }
}