using Autofac;
using SmartRecipes.Mobile.Pages;
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

            // Read models
            builder.RegisterType<ShoppingListRepository>();

            // Write models
            builder.RegisterType<ShoppingListHandler>();

            // View models
            builder.RegisterType<SignInViewModel>();
            builder.RegisterType<ShoppingListItemsViewModel>();
            builder.RegisterType<AddShoppingListItemViewModel>();
            builder.RegisterType<MyRecipesViewModel>();

            // Pages
            builder.RegisterType<SignInPage>();
            builder.RegisterType<SignUpPage>();
            builder.RegisterType<AppContainer>();
            builder.RegisterType<ShoppingListItemsPage>();
            builder.RegisterType<AddShoppingListItemPage>();
            builder.RegisterType<MyRecipesPage>();

            return builder.Build();
        }
    }
}
