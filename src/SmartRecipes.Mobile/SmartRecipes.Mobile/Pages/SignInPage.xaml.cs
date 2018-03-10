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

            EmailEntry.SetBinding(Entry.TextProperty, nameof(viewModel.Email));
            PasswordEntry.SetBinding(Entry.TextProperty, nameof(viewModel.Password));

            SignInButton.Command = new Command(() => viewModel.SignIn());
            SignUpButton.Command = new Command(() => viewModel.NavigateToSignUp());
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            ScaleBackground(width, height);
        }

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