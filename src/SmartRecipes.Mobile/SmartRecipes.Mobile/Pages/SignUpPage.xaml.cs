using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartRecipes.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignUpPage : ContentPage
	{
		public SignUpPage ()
		{
			InitializeComponent ();
		    SignInButton.Clicked += (sender, args) =>
		    {
		        Application.Current.MainPage = new SignInPage();
		    };

		}
	}
}