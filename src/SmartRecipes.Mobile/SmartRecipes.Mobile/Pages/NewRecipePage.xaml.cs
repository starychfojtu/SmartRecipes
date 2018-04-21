using Xamarin.Forms;

namespace SmartRecipes.Mobile.Pages
{
    public partial class NewRecipePage : ContentPage
    {
        public NewRecipePage(NewRecipeViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            NameEntry.SetBinding(Entry.TextProperty, $"{nameof(viewModel.Recipe)}.{nameof(NewRecipeViewModel.FormDto.Name)}");
            ImageUrlEntry.SetBinding(Entry.TextProperty, $"{nameof(viewModel.Recipe)}.{nameof(NewRecipeViewModel.FormDto.ImageUrl)}");
            PersonCountEntry.SetBinding(Entry.TextProperty, $"{nameof(viewModel.Recipe)}.{nameof(NewRecipeViewModel.FormDto.PersonCount)}");
            TextEditor.SetBinding(Editor.TextProperty, $"{nameof(viewModel.Recipe)}.{nameof(NewRecipeViewModel.FormDto.Text)}");
        }
    }
}
