namespace AMTools.Web.Data.JsonStore.Services.Interfaces
{
    public interface IJsonStoreService
    {
        bool ConfigIsValid();
        void EmptyAllJsonStores();
    }
}