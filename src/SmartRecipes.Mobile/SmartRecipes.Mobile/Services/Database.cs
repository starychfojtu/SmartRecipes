using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public class Database
    {
        private const string FileName = "SmartRecies.db";

        private SQLiteAsyncConnection connection;

        public Database()
        {
        }

        public async Task UpdateAsync(Recipe recipe)
        {
            await Connection.UpdateAsync(recipe);
        }

        public async Task UpdateAsync(Ingredient ingredient)
        {
            await Connection.UpdateAsync(ingredient);
        }

        public async Task UpdateAsync(Foodstuff foodstuff)
        {
            await Connection.UpdateAsync(foodstuff);
        }

        public AsyncTableQuery<Recipe> Recipes
        {
            get { return Connection.Table<Recipe>(); }
        }

        public AsyncTableQuery<Ingredient> Ingredients
        {
            get { return Connection.Table<Ingredient>(); }
        }

        public AsyncTableQuery<Foodstuff> Foodstuffs
        {
            get { return Connection.Table<Foodstuff>(); }
        }

        private SQLiteAsyncConnection Connection
        {
            get { return connection ?? (connection = InitializeDb()); }
        }

        private SQLiteAsyncConnection InitializeDb()
        {
            var conn = new SQLiteAsyncConnection(DependencyService.Get<IFileHelper>().GetLocalFilePath(FileName));
            conn.CreateTableAsync<Recipe>().Wait();
            conn.CreateTableAsync<Ingredient>().Wait();
            conn.CreateTableAsync<Foodstuff>().Wait();
            return conn;
        }
    }
}
