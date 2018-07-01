using System.Collections.Generic;
using System.Linq;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Views
{
    public partial class RecipeCell : ViewCell
    {
        public RecipeCell()
        {
            InitializeComponent();
            Image.Transformations.Add(new CircleTransformation());
        }

        private RecipeCellViewModel ViewModel => BindingContext as RecipeCellViewModel;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Reset();

            if (ViewModel != null)
            {
                var recipe = ViewModel.Detail.Recipe;
                var thumbnails = ViewModel.Detail.Ingredients.Select(i => Controls.Image.Thumbnail(i.Foodstuff.ImageUrl));
                var newActionButtons = ViewModel.Actions.OrderBy(a => a.Order).Select(a =>
                {
                    return Controls.Controls.ActionButton(a.Icon).Tee(b => 
                        b.Clicked += async (s, e) => await UserMessage.PopupAction(() => a.Action(recipe))
                    );
                });
                
                NameLabel.Text = recipe.Name;
                Image.Source = ImageSource.FromUri(recipe.ImageUrl);
                PersonCount.Text = ViewModel.PersonCount.ToString();
                ActionContainer.Children.AddRange(newActionButtons);
                IngredientsStackLayout.Children.AddRange(thumbnails);
            }
        }

        private void Reset()
        {
            NameLabel.Text = "";
            Image.Source = null;
            PersonCount.Text = "";
            ActionContainer.Children.Clear();
            IngredientsStackLayout.Children.Clear();
        }
    }
}
