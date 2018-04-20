using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartRecipes.Mobile
{
    public class Database
    {
        private const string FileName = "SmartRecies.db";

        private SQLiteAsyncConnection connection;

        public Database()
        {
        }

        public async Task AddAsync<T>(IEnumerable<T> recipes)
        {
            await Connection.InsertAllAsync(recipes);
        }

        public async Task UpdateAsync<T>(T item)
        {
            await Connection.UpdateAsync(item);
        }

        public async Task AddOrReplaceAsync<T>(T item)
        {
            await Connection.InsertOrReplaceAsync(item);
        }

        public AsyncTableQuery<Recipe> Recipes
        {
            get { return Connection.Table<Recipe>(); }
        }

        public AsyncTableQuery<FoodstuffAmount> FoodstuffAmounts
        {
            get { return Connection.Table<FoodstuffAmount>(); }
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
            conn.CreateTableAsync<FoodstuffAmount>().Wait();
            conn.CreateTableAsync<Foodstuff>().Wait();
            return conn;
        }
    }
}
