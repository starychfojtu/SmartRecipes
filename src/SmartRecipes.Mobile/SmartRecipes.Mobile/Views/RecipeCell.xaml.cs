using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Views
{
    public partial class RecipeCell : ViewCell
    {
        public RecipeCell()
        {
            InitializeComponent();

            OtherButton.Clicked += (s, e) => ViewModel?.OnOther?.Invoke();
            PlusButton.Clicked += async (s, e) => await ViewModel?.OnPlus.Invoke();
            EditButton.Clicked += async (s, e) => await ViewModel?.EditRecipe();
        }

        private RecipeCellViewModel ViewModel => (BindingContext as RecipeCellViewModel);

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
                //var thumbnails = ingredients.Select(i => Image.Thumbnail(i.Foodstuff.ImageUrl));
                //IngredientsStackLayout.Children.AddRange(thumbnails);
            }
        }
    }
}
