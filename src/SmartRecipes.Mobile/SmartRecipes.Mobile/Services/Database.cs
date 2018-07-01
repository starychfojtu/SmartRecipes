using SQLite;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using LanguageExt;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.Services
{
    public class Database
    {
        private const string FileName = "SmartRecies.db";

        private SQLiteAsyncConnection connection;

        public Task<Unit> AddAsync<T>(IEnumerable<T> items)
        {
            return Connection.InsertAllAsync(items).ToUnitTask();
        }

        public Task<Unit> UpdateAsync<T>(IEnumerable<T> items)
        {
            return Connection.UpdateAllAsync(items).ToUnitTask();
        }

        public Task<Unit> UpdateAsync<T>(T item)
        {
            return Connection.UpdateAsync(item).ToUnitTask();
        }

        public Task<Unit> AddOrReplaceAsync<T>(T item)
        {
            return Connection.InsertOrReplaceAsync(item).ToUnitTask();
        }

        public Task<Unit> Delete<T>(T item)
        {
            return Connection.DeleteAsync(item).ToUnitTask();
        }

        public Task<IEnumerable<T>> Query<T>(string sql, params object[] args)
            where T : new()
        {
            return connection.QueryAsync<T>(sql, args).Map(t => (IEnumerable<T>)t);
        }

        public Task<Unit> Execute(string sql, params object[] args)
        {
            return connection.QueryAsync<int>(sql, args).ToUnitTask();
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
        
        public Task<Unit> Seed()
        {
            var foodstuffs = Foodstuffs.CountAsync().Bind(c => c == 0 ? AddAsync(FakeData.FakeFoodstuffs()) : Task.FromResult(Unit.Default));
            var recipes = Recipes.CountAsync().Bind(c => c == 0 ? AddAsync(FakeData.FakeRecipes()) : Task.FromResult(Unit.Default));

            return foodstuffs.Bind(_ => recipes);
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
