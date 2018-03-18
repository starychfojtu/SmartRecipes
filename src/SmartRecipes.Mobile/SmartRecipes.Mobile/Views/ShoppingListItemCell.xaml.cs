using Xamarin.Forms;
using FFImageLoading.Transformations;

namespace SmartRecipes.Mobile.Views
{
    public partial class ShoppingListItemCell : ViewCell
    {
        public ShoppingListItemCell()
        {
            InitializeComponent();

            Image.Transformations.Add(new CircleTransformation());

            DecreaseAmountButton.Clicked += (s, e) => DecreaseAmount();
            IncreaseAmountButton.Clicked += (s, e) => IncreaseAmount();
        }

        public ShoppingListItem Item
        {
            get { return BindingContext as ShoppingListItem; }
        }

        private void DecreaseAmount()
        {
            Item.DecreaseAmount(); // TODO: Make viewmodel for this cell and call shoppingList method
            OnBindingContextChanged(); // TODO: Refactor when found a better way by binding
        }

        private void IncreaseAmount()
        {
            Item.IncreaseAmount();
            OnBindingContextChanged(); // TODO: Refactor when found a better way by binding
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (Item != null)
            {
                NameLabel.Text = Item.Foodstuff.Name;
                AmountLabel.Text = Item.Amount.ToString();
                Image.Source = Item.Foodstuff.ImageUrl.AbsoluteUri;
            }
        }
    }
}
