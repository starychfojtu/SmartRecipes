using SmartRecipes.Mobile.Pages;
using Xamarin.Forms;
using SmartRecipes.Mobile.Services;
using System.Net;

namespace SmartRecipes.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            Current.MainPage = await PageFactory.GetPageAsync<SignInPage>();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}
