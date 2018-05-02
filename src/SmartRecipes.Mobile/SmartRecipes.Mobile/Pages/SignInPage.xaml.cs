using SmartRecipes.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

            viewModel.BindErrors(EmailEntry, vm => vm.Email.IsValid);
            viewModel.BindText(EmailEntry, vm => vm.Email.Data);

            viewModel.BindErrors(PasswordEntry, vm => vm.Password.IsValid);
            viewModel.BindText(PasswordEntry, vm => vm.Password.Data);

            SignInButton.Clicked += async (s, e) =>
            {
                if (!await viewModel.SignIn())
                {
                    await DisplayAlert("Sign in failed.", "Email or password is incorrect.", "Ok");
                }
            };
            SignUpButton.Clicked += async (s, e) => await viewModel.SignUp();
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