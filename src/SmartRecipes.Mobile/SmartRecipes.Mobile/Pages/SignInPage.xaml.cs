using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartRecipes.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPage : ContentPage
    {
        public SignInPage()
        {
            InitializeComponent();

            SignUpButton.Clicked += (s, a) =>
            {
                Application.Current.MainPage = new SignUpPage();
            };
        }
    }
}