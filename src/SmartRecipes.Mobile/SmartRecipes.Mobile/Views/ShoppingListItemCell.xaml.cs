using Xamarin.Forms;

namespace SmartRecipes.Mobile.Views
{
    public partial class ShoppingListItemCell : ViewCell
    {
        public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(ShoppingListItemCell), defaultValue: "Name");
        public static readonly BindableProperty AmountProperty = BindableProperty.Create(nameof(Amount), typeof(string), typeof(ShoppingListItemCell), defaultValue: "Amount");
        public static readonly BindableProperty ImageUrlProperty = BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(ShoppingListItemCell), defaultValue: "");

        public ShoppingListItemCell()
        {
            InitializeComponent();
        }

        // Change name to simple binding, because it wont be changed from the cell
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public string Amount
        {
            get { return (string)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                NameLabel.Text = Name;
                AmountLabel.Text = Amount;
                Image.Source = ImageUrl;
            }
        }
    }
}
