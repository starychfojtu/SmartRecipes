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

            EmailEntry.SetBinding(Entry.TextProperty, nameof(viewModel.Email));
            PasswordEntry.SetBinding(Entry.TextProperty, nameof(viewModel.Password));

            SignInButton.Command = new Command(() => viewModel.SignIn());
            SignUpButton.Command = new Command(() => viewModel.NavigateToSignUp());
        }

        public SignInPage() // For live page
        {
            InitializeComponent();
        }
    }
}