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

        public async Task<int> AddAsync<T>(IEnumerable<T> items)
        {
            return await Connection.InsertAllAsync(items);
        }

        public async Task<int> UpdateAsync<T>(IEnumerable<T> items)
        {
            return await Connection.UpdateAllAsync(items);
        }

        public async Task<int> AddOrReplaceAsync<T>(T item)
        {
            return await Connection.InsertOrReplaceAsync(item);
        }

        public async Task<int> Delete<T>(T item)
        {
            return await Connection.DeleteAsync(item);
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

        public AsyncTableQuery<IngredientAmount> IngredientAmounts
        {
            get { return Connection.Table<IngredientAmount>(); }
        }

        public AsyncTableQuery<ShoppingListItemAmount> ShoppingListItemAmounts
        {
            get { return Connection.Table<ShoppingListItemAmount>(); }
        }

        public AsyncTableQuery<Foodstuff> Foodstuffs
        {
            get { return Connection.Table<Foodstuff>(); }
        }

        public AsyncTableQuery<RecipeInShoppingList> RecipeInShoppingLists
        {
            get { return Connection.Table<RecipeInShoppingList>(); }
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
            syncConn.CreateTable<IngredientAmount>();
            syncConn.CreateTable<ShoppingListItemAmount>();
            syncConn.CreateTable<Foodstuff>();
            syncConn.CreateTable<RecipeInShoppingList>();

            return conn;
        }
    }
}
