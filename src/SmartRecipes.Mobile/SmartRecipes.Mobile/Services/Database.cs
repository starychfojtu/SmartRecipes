using SQLite;
using Xamarin.Forms;

namespace SmartRecipes.Mobile
{
    public class Database
    {
        private const string FileName = "SmartRecies.db";

        private SQLiteAsyncConnection connection;

        private SQLiteAsyncConnection Connection
        {
            get { return connection ?? (connection = InitializeDb()); }
        }

        public AsyncTableQuery<Recipe> Recipes
        {
            get { return connection.Table<Recipe>(); }
        }

        public AsyncTableQuery<Ingredient> Ingredients
        {
            get { return connection.Table<Ingredient>(); }
        }

        public AsyncTableQuery<Foodstuff> Foodstuffs
        {
            get { return connection.Table<Foodstuff>(); }
        }

        private SQLiteAsyncConnection InitializeDb()
        {
            var conn = new SQLiteAsyncConnection(DependencyService.Get<IFileHelper>().GetLocalFilePath(FileName));
            conn.CreateTableAsync<Recipe>();
            conn.CreateTableAsync<Ingredient>();
            conn.CreateTableAsync<Foodstuff>();
            return conn;
        }
    }
}
