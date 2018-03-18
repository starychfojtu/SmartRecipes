using Autofac;
using SmartRecipes.Mobile.Pages;
using System.Net.Http;
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
            builder.RegisterType<Authenticator>();
            builder.RegisterType<ShoppingList>().SingleInstance();

            // View models
            builder.RegisterType<SignInViewModel>();
            builder.RegisterType<ShoppingListItemsViewModel>();

            // Pages
            builder.RegisterType<SignInPage>();
            builder.RegisterType<SignUpPage>();
            builder.RegisterType<ShoppingListPage>();
            builder.RegisterType<ShoppingListItemsPage>();
            builder.RegisterType<AddShoppingListItemPage>();

            return builder.Build();
        }
    }
}
