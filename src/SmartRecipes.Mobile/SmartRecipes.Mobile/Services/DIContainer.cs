using Autofac;
using System.Net.Http;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Services
{
    public class DIContainer
    {
        private static IContainer instance;

        private DIContainer()
        {
        }

        public static IContainer Instance
        {
            get { return instance ?? (instance = Initialize()); }
        }

        private static IContainer Initialize()
        {
            var builder = new ContainerBuilder();

            // Services
            builder.RegisterInstance(new HttpClient()).As<HttpClient>();
            builder.RegisterType<ApiClient>().SingleInstance();
            builder.RegisterType<Database>().SingleInstance();

            // Read models
            builder.RegisterType<ShoppingListRepository>();
            builder.RegisterType<RecipeRepository>();
            builder.RegisterType<FoodstuffRepository>();

            // Write models
            builder.RegisterType<ShoppingListHandler>();
            builder.RegisterType<UserHandler>();
            builder.RegisterType<NewRecipeViewModel>();

            // View models
            builder.RegisterType<SignInViewModel>();
            builder.RegisterType<ShoppingListItemsViewModel>();
            builder.RegisterType<FoodstuffSearchViewModel>();
            builder.RegisterType<MyRecipesViewModel>();

            return builder.Build();
        }
    }
}
