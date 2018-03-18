using System.Collections.Generic;
using System.Linq;

namespace SmartRecipes.Mobile
{
    public class ShoppingList
    {
        private readonly ApiClient apiClient;

        private IEnumerable<ShoppingListItem> items;

        private bool hasStaleData = true;

        public ShoppingList(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public IEnumerable<ShoppingListItem> GetItems()
        {
            return hasStaleData ? (items = LoadItems()) : items;
        }

        public void IncreaseAmount(ShoppingListItem item)
        {
            item.IncreaseAmount();
            // TODO: Call API
        }

        public void DecreaseAmount(ShoppingListItem item)
        {
            item.DecreaseAmount();
            // TODO: Call API
        }

        private IEnumerable<ShoppingListItem> LoadItems()
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
