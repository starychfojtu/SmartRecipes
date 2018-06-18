using SmartRecipes.Mobile.Pages;
using Xamarin.Forms;
using SmartRecipes.Mobile.Services;

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
    }
}
