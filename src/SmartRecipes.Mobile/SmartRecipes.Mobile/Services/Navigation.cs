using Xamarin.Forms;
using SmartRecipes.Mobile.Pages;
using System.Threading.Tasks;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ViewModels;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using System;
using System.Collections;

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

        public static async Task AddRecipe(MyRecipesViewModel viewModel)
        {
            var page = await PageFactory.GetPageAsync<NewRecipePage>();
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
