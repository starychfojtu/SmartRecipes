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

        public ShoppingListItemCellViewModel ViewModel
        {
            get { return BindingContext as ShoppingListItemCellViewModel; }
        }

        private void DecreaseAmount()
        {
            ViewModel.Store.DecreaseAmount(ViewModel.Item);
            OnBindingContextChanged(); // TODO: Refactor when found a better way by binding
        }

        private void IncreaseAmount()
        {
            ViewModel.Store.IncreaseAmount(ViewModel.Item);
            OnBindingContextChanged(); // TODO: Refactor when found a better way by binding
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                NameLabel.Text = ViewModel.Item.Foodstuff.Name;
                AmountLabel.Text = ViewModel.Item.Amount.ToString();
                Image.Source = ViewModel.Item.Foodstuff.ImageUrl.AbsoluteUri;
            }
        }
    }
}
