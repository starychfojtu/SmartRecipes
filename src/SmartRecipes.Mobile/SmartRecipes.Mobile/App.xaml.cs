using SmartRecipes.Mobile.Pages;
using Xamarin.Forms;
using Autofac;

namespace SmartRecipes.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Current.MainPage = DIContainer.Instance.Resolve<SignInPage>();
        }

        protected override void OnStart()
        {
            base.OnStart();
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
