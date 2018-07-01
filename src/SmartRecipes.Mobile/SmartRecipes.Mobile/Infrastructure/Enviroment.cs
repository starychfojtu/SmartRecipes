namespace SmartRecipes.Mobile.Infrastructure
{
    public class Enviroment
    {
        public Enviroment(ApiClient apiClient, Database database)
        {
            Api = apiClient;
            Db = database;
        }

        public ApiClient Api { get; }

        public Database Db { get; }
    }
}
