using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.Services
{
    public class Database
    {
        private const string FileName = "SmartRecies.db";

        private SQLiteAsyncConnection connection;

        public async Task AddAsync<T>(IEnumerable<T> items)
        {
            await Connection.InsertAllAsync(items);
        }

        public async Task UpdateAsync<T>(IEnumerable<T> items)
        {
            await Connection.UpdateAllAsync(items);
        }

        public async Task AddOrReplaceAsync<T>(T item)
        {
            await Connection.InsertOrReplaceAsync(item);
        }

        public async Task<IEnumerable<T>> Execute<T>(string sql, params object[] args)
            where T : new()
        {
            return await connection.QueryAsync<T>(sql, args);
        }

        public TableMapping GetTableMapping<T>()
        {
            return connection.GetConnection().GetMapping<T>();
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
            var syncConn = conn.GetConnection();
            syncConn.CreateTable<Recipe>();
            syncConn.CreateTable<FoodstuffAmount>();
            syncConn.CreateTable<Foodstuff>();
            return conn;
        }
    }
}
