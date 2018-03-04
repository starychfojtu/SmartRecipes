using SmartRecipes.Mobile.Pages;
using Xamarin.Forms;

namespace SmartRecipes.Mobile
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

		    Application.Current.MainPage = new SignInPage();
		}

		protected override void OnStart ()
		{
		    base.OnStart();
		}

		protected override void OnSleep ()
		{
		    base.OnSleep();
        }

		protected override void OnResume ()
		{
			base.OnResume();
		}
	}
}
