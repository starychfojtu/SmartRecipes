using System.Collections.Generic;
using System.Linq;

namespace SmartRecipes.Mobile
{
    /// <summary>
    /// The only place where mutable transactions, handles state.
    /// </summary>
    public partial class Store
    {
        private readonly ApiClient apiClient;

        private IList<ShoppingListItem> shoppingListItems;

        public Store(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public IList<ShoppingListItem> ShoppingListItems
        {
            get
            {
                return shoppingListItems ?? (shoppingListItems = GetShoppingListItems().ToList());
            }
        }

        public void DecreaseAmount(ShoppingListItem item)
        {
            var newItem = ShoppingListItem.DecreaseAmount(item);
            shoppingListItems = shoppingListItems.Replpace(item, newItem);
            // TODO: construct request and send API call
        }

        public void IncreaseAmount(ShoppingListItem item)
        {
            var newItem = ShoppingListItem.IncreaseAmount(item);
            shoppingListItems = shoppingListItems.Replpace(item, newItem);
            // TODO: construct request and send API call
        }

        private IEnumerable<ShoppingListItem> GetShoppingListItems()
        {
            return apiClient.GetShoppingList().Items.Select(i =>
                new ShoppingListItem(
                    new Foodstuff(
                        i.FoodstuffDto.Id,
                        i.FoodstuffDto.Name,
                        i.FoodstuffDto.ImageUrl,
                        i.FoodstuffDto.BaseAmount,
                        i.FoodstuffDto.AmountStep
                    ),
                    i.Amount
                )
             );
        }

    }
}
