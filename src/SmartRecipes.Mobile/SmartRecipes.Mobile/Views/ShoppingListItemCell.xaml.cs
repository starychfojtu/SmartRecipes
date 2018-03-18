using Xamarin.Forms;
using FFImageLoading.Transformations;

namespace SmartRecipes.Mobile.Views
{
    public partial class ShoppingListItemCell : ViewCell
    {
        public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), typeof(ShoppingListItem), typeof(ShoppingListItemCell), defaultValue: null);

        public ShoppingListItemCell()
        {
            InitializeComponent();
        }

        public ShoppingListItem Item
        {
            get { return (ShoppingListItem)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        private void DecreaseAmount()
        {
            Item.Amount.Substract(Item.Foodstuff.AmountStep);
        }

        private void IncreaseAmount()
        {
            Item.Amount.Add(Item.Foodstuff.AmountStep);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                NameLabel.Text = Item.Foodstuff.Name;
                AmountLabel.Text = Item.Amount.ToString();

                Image.Source = Item.Foodstuff.ImageUrl;
                Image.Transformations.Add(new CircleTransformation());

                DecreaseAmountButton.Clicked += (s, e) => DecreaseAmount();
                IncreaseAmountButton.Clicked += (s, e) => IncreaseAmount();
            }
        }
    }
}
