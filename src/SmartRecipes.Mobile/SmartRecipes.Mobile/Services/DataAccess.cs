namespace SmartRecipes.Mobile.Services
{
    public class DataAccess
    {
        public DataAccess(ApiClient apiClient, Database database)
        {
            Api = apiClient;
            Db = database;
        }

        public ApiClient Api { get; }

        public Database Db { get; }
    }
}
