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

        private SQLiteAsyncConnection Connection
        {
            get { return connection ?? (connection = InitializeDb()); }
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
