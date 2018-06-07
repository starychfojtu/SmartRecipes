using LanguageExt;
using SmartRecipes.Mobile.Models;
using LanguageExt.SomeHelp;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class ShoppingListItem
    {
        public ShoppingListItem(Some<IFoodstuff> foodstuff, Some<IShoppingListItemAmount> itemAmount)
        {
            Foodstuff = foodstuff.Value;
            ItemAmount = itemAmount.Value;
        }

        public IFoodstuff Foodstuff { get; }

        public IShoppingListItemAmount ItemAmount { get; }

        public IAmount Amount
        {
            get { return ItemAmount.Amount; }
        }

        public ShoppingListItem WithItemAmount(Some<IShoppingListItemAmount> itemAmount)
        {
            return new ShoppingListItem(Foodstuff.ToSome(), itemAmount.ToSome());
        }
    }
}
