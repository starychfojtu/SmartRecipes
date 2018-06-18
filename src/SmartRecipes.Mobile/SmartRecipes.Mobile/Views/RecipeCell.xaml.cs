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

            if (ViewModel != null)
            {
                var newActionButtons = ViewModel.Actions.OrderBy(a => a.Order).Select(a =>
                {
                    return Controls.Controls.ActionButton(a.Icon).Tee(b => 
                        b.Clicked += async (s, e) => await UserMessage.PopupAction(() => a.Action(ViewModel.Recipe))
                    );
                });
                
                NameLabel.Text = ViewModel.Recipe.Name;
                Image.Source = ImageSource.FromUri(ViewModel.Recipe.ImageUrl);
                ReplaceActions(newActionButtons);
                
                //var thumbnails = ingredients.Select(i => Image.Thumbnail(i.Foodstuff.ImageUrl));
                var t1 = Controls.Image.Thumbnail(ViewModel.Recipe.ImageUrl);
                var t2 = Controls.Image.Thumbnail(ViewModel.Recipe.ImageUrl);
                ReplaceIngredients(new [] {t1, t2});
            }
        }

        private void ReplaceActions(IEnumerable<Button> buttons)
        {
            ActionContainer.Children.Clear();
            ActionContainer.Children.AddRange(buttons);
        }
        
        private void ReplaceIngredients(IEnumerable<CachedImage> thumbnails)
        {
            IngredientsStackLayout.Children.Clear();
            IngredientsStackLayout.Children.AddRange(thumbnails);
        }
    }
}
