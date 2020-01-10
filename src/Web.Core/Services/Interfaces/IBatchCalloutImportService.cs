namespace AMTools.Web.Core.Services.Interfaces
{
    public interface IBatchCalloutImportService
    {
        void Import(string inputDirectory, string inputFilenamePattern);
    }
}