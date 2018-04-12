using Autofac;
using System.Net.Http;
using SmartRecipes.Mobile.Controllers;
using SmartRecipes.Mobile.ReadModels;

namespace SmartRecipes.Mobile
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

            // Write models
            builder.RegisterType<ShoppingListHandler>();
            builder.RegisterType<SecurityHandler>();
            builder.RegisterType<MyRecipesHandler>();

            // View models
            builder.RegisterType<SignInViewModel>();
            builder.RegisterType<ShoppingListItemsViewModel>();
            builder.RegisterType<AddShoppingListItemViewModel>();
            builder.RegisterType<MyRecipesViewModel>();

            return builder.Build();
        }
    }
}
