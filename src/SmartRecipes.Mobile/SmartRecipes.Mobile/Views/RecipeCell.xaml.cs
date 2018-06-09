using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.Views
{
    public partial class RecipeCell : ViewCell
    {
        public RecipeCell()
        {
            InitializeComponent();

            OtherButton.Clicked += (s, e) => OnOther();
            PlusButton.Clicked += (s, e) => OnPlus();

            EditButton.Clicked += async (s, e) =>
            {
                if (ViewModel != null)
                {
                    await Navigation.EditRecipe(ViewModel.Recipe);
                }
            };
        }

        private RecipeCellViewModel ViewModel => (BindingContext as RecipeCellViewModel);

        private void OnOther()
        {
            ViewModel.OnOther?.Invoke();
        }

        private void OnPlus()
        {
            ViewModel.OnPlus.Invoke();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                var recipe = ViewModel.Recipe;
                NameLabel.Text = recipe.Name;
                OtherButton.IsVisible = ViewModel.OnOther != null;
                IngredientsStackLayout.Children.Clear();

                // TODO: in future versions
                //var thumbnails = ingredients.Select(i => Image.Thumbnail(i.Value.Foodstuff.ImageUrl));
                //IngredientsStackLayout.Children.AddRange(thumbnails);
            }
        }
    }
}
