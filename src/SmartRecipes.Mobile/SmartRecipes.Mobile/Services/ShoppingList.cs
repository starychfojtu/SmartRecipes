using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace SmartRecipes.Mobile
{
    public class ShoppingList
    {
        private readonly ApiClient apiClient;

        public ShoppingList(ApiClient apiClient)
        {
            this.apiClient = apiClient;
            IsValid = false;
            Items = new List<ShoppingListItem>();
        }

        private bool IsValid { get; set; }

        private IEnumerable<ShoppingListItem> Items { get; set; }

        public IEnumerable<ShoppingListItem> GetItems()
        {
            LoadData();
            return Items;
        }

        private void LoadData()
        {
            if (!IsValid)
            {
                var response = apiClient.GetShoppingList();

                Items = response.Items.Select(i => new ShoppingListItem(new Foodstuff(i.FoodstuffDto.Id, i.FoodstuffDto.Name, i.FoodstuffDto.ImageUrl), i.Amount));
            }
        }
    }
}
