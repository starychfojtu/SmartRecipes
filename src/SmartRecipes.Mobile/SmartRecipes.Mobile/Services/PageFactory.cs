using System;
using System.Threading.Tasks;
using Autofac;
using SmartRecipes.Mobile.Pages;
using System.Collections.Generic;
using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;
using System.Collections.Immutable;

namespace SmartRecipes.Mobile.Services
{
    public static class PageFactory
    {
        private static IImmutableDictionary<Type, Type> ViewModelByPages { get; }

        static PageFactory()
        {
            var types = new Dictionary<Type, Type>
            {
                { typeof(SignInPage), typeof(SignInViewModel) },
                { typeof(ShoppingListItemsPage), typeof(ShoppingListItemsViewModel) },
                { typeof(FoodstuffSearchPage), typeof(FoodstuffSearchViewModel) },
                { typeof(MyRecipesPage), typeof(MyRecipesViewModel) },
                { typeof(NewRecipePage), typeof(NewRecipeViewModel) }
            };
            ViewModelByPages = types.ToImmutableDictionary();
        }

        public static async Task<T> GetPageAsync<T>() where T : class
        {
            var pageType = typeof(T);
            if (pageType == typeof(AppContainer))
            {
                var appContainer = new AppContainer(
                    await GetPageAsync<ShoppingListItemsPage>(),
                    await GetPageAsync<MyRecipesPage>()
                );
                return appContainer as T;
            }

            var viewModelType = ViewModelByPages[pageType];
            var viewModel = (ViewModel)DIContainer.Instance.Resolve(viewModelType);
            await viewModel.InitializeAsync();
            return Activator.CreateInstance(pageType, viewModel) as T;
        }

        public static TabbedPage CreateTabbed(string title, params Page[] pages)
        {
            var tabbedPage = new TabbedPage { Title = title };
            return tabbedPage.Tee(p => p.Children.AddRange(pages));
        }
    }
}
