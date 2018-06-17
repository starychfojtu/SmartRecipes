using System.Collections.Immutable;
using System.Linq;
using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Views
{
    public partial class RecipeCell : ViewCell
    {
        private IImmutableList<Button> actionButtons;
        
        public RecipeCell()
        {
            InitializeComponent();
            EditButton.Clicked += async (s, e) => await ViewModel.EditRecipe();
        }

        private RecipeCellViewModel ViewModel => BindingContext as RecipeCellViewModel;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                var newActionButtons = ViewModel.Actions.OrderBy(a => a.Order).Select(a =>
                {
                    var actionButton = new Button
                    {
                        HeightRequest = 64,
                        WidthRequest = 64,
                        Image = a.Icon.ImageName,
                        VerticalOptions = LayoutOptions.Center,
                        BackgroundColor = Color.Transparent
                    };
                    actionButton.Clicked += async (s, e) => await a.Action(ViewModel.Recipe);
                    return actionButton;
                });

                NameLabel.Text = ViewModel.Recipe.Name;
                
                actionButtons.Iter(b => MainLayout.Children.Remove(b));
                actionButtons = newActionButtons.ToImmutableList();
                MainLayout.Children.AddRange(actionButtons);

                // TODO: in future versions
                // IngredientsStackLayout.Children.Clear();
                //var thumbnails = ingredients.Select(i => Image.Thumbnail(i.Foodstuff.ImageUrl));
                //IngredientsStackLayout.Children.AddRange(thumbnails);
            }
        }
    }
}
