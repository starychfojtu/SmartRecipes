using Xamarin.Forms;

namespace SmartRecipes.Mobile
{
    public partial class NewRecipePage : ContentPage
    {
        public NewRecipePage(NewRecipeViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}
