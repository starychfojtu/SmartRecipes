using SmartRecipes.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SmartRecipes.Mobile.Controls;

namespace SmartRecipes.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPage : ContentPage
    {
        private const double BackgroundHeight = 2057d;
        private const double BackgroundWidth = 1360d;

        public SignInPage(SignInViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            viewModel.Bind(EmailEntry, Entry.TextProperty, vm => vm.Email.Data);
            viewModel.Bind(EmailEntry, ValidatableEntry.ErrorsProperty, vm => vm.Email.Errors);
            viewModel.Bind(PasswordEntry, Entry.TextProperty, vm => vm.Password);

            SignInButton.Clicked += (s, e) => viewModel.SignIn();
            SignUpButton.Clicked += (s, e) => viewModel.SignUp();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            ScaleBackground(width, height);
        }

        // TODO: move to generic placeholder
        private void ScaleBackground(double width, double height)
        {
            var backgroundAspectRatio = BackgroundWidth / BackgroundHeight;
            var screenAspectRation = width / height;
            var errorDeviation = 0.05;

            if (screenAspectRation > backgroundAspectRatio)
            {
                var aspectedHeight = (BackgroundHeight / BackgroundWidth) * width;
                BackgroundImage.Scale = errorDeviation + (aspectedHeight / height);
            }
            else
            {
                var aspectedWidth = backgroundAspectRatio * height;
                BackgroundImage.Scale = errorDeviation + (aspectedWidth / width);
            }
        }
    }
}