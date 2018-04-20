using Xamarin.Forms;

namespace SmartRecipes.Mobile
{
    public partial class NewRecipePage : ContentPage
    {
        public NewRecipePage(NewRecipeViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            NameEntry.SetBinding(Entry.TextProperty, $"{nameof(viewModel.RecipeDto)}.{nameof(NewRecipeViewModel.FormDto.Name)}");
            ImageUrlEntry.SetBinding(Entry.TextProperty, $"{nameof(viewModel.RecipeDto)}.{nameof(NewRecipeViewModel.FormDto.ImageUrl)}");
            PersonCountEntry.SetBinding(Entry.TextProperty, $"{nameof(viewModel.RecipeDto)}.{nameof(NewRecipeViewModel.FormDto.PersonCount)}");
            TextEditor.SetBinding(Editor.TextProperty, $"{nameof(viewModel.RecipeDto)}.{nameof(NewRecipeViewModel.FormDto.Text)}");
        }
    }
}
