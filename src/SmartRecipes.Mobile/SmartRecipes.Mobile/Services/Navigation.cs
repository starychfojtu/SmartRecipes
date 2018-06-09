using Xamarin.Forms;
using SmartRecipes.Mobile.Pages;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;

namespace SmartRecipes.Mobile.Services
{
    public static class Navigation
    {
        public static async Task LogIn()
        {
            Application.Current.MainPage = new NavigationPage(await PageFactory.GetPageAsync<AppContainer>());
        }

        public static async Task SignUp()
        {
            Application.Current.MainPage = await PageFactory.GetPageAsync<SignUpPage>();
        }

        public static async Task CreateRecipe()
        {
            var page = await PageFactory.GetPageAsync<EditRecipePage>();
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public static async Task EditRecipe(IRecipe recipe)
        {
            var page = await PageFactory.GetEditRecipePage(recipe);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public static async Task<IEnumerable<IFoodstuff>> SelectFoodstuffDialog()
        {
            var selected = new TaskCompletionSource<IEnumerable<IFoodstuff>>();
            var page = await PageFactory.GetPageAsync<FoodstuffSearchPage>();

            page.SelectingEnded += (s, e) => selected.SetResult(e.Selected);

            await Application.Current.MainPage.Navigation.PushAsync(page);

            return await selected.Task;
        }
    }
}
