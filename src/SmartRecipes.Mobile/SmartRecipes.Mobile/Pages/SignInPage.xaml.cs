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
	public partial class SignInPage : ContentPage
	{
		public SignInPage ()
		{
			InitializeComponent ();

		    SignUpButton.Clicked += (sender, args) =>
		    {
		        Application.Current.MainPage = new ContentPage();
		    };
		}
	}
}