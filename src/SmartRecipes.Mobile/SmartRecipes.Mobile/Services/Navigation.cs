using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;
using System.Threading.Tasks;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Services
{
    public static class Navigation
    {
        public static async Task LogIn(SecurityHandler controller)
        {
            Application.Current.MainPage = new NavigationPage(await PageFactory.GetPageAsync<AppContainer>());
        }

        public static async Task SignUp(SignInViewModel viewModel)
        {
            Application.Current.MainPage = await PageFactory.GetPageAsync<SignUpPage>();
        }

        public static async Task AddShoppingListItem(ShoppingListItemsViewModel viewModel)
        {
            var page = await PageFactory.GetPageAsync<AddShoppingListItemPage>();
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public static async Task AddRecipe(MyRecipesViewModel viewModel)
        {
            var page = await PageFactory.GetPageAsync<NewRecipePage>();
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}
