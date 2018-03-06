using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartRecipes.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPage : ContentPage
    {
        public SignInPage(SignInViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            EmailEntry.SetBinding(Entry.TextProperty, nameof(viewModel.Credentials.Email));
            PasswordEntry.SetBinding(Entry.TextProperty, nameof(viewModel.Credentials.Password));

            SignInButton.Command = new Command(() => viewModel.SignIn());
            SignUpButton.Command = new Command(() => viewModel.NavigateToSignUp());
        }
    }
}