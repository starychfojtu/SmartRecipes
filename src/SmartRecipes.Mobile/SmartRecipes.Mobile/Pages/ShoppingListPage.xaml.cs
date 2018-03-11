using Autofac;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Pages
{
    public partial class ShoppingListPage : TabbedPage
    {
        public ShoppingListPage()
        {
            InitializeComponent();

            Children.Add(DIContainer.Instance.Resolve<ShoppingListItemsPage>());
        }
    }
}
