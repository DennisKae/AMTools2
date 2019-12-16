namespace AMTools.Web.Data.Database.Repositories
{
    public class BaseRepository
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly DatabaseContext _databaseContext;
#pragma warning restore IDE0052 // Remove unread private members
        public BaseRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
    }
}
