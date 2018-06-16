using LanguageExt;
using SmartRecipes.Mobile.Models;
using LanguageExt.SomeHelp;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class ShoppingListItem
    {
        public ShoppingListItem(IFoodstuff foodstuff, IShoppingListItemAmount itemAmount)
        {
            Foodstuff = foodstuff;
            ItemAmount = itemAmount;
        }

        public IFoodstuff Foodstuff { get; }

        public IShoppingListItemAmount ItemAmount { get; }

        public IAmount Amount
        {
            get { return ItemAmount.Amount; }
        }

        public ShoppingListItem WithItemAmount(IShoppingListItemAmount itemAmount)
        {
            return new ShoppingListItem(Foodstuff, itemAmount);
        }
    }
}
