using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using System.Reactive.Linq;

namespace SmartRecipes.Mobile
{
    public partial class Store
    {
        private IList<Ingredient> shoppingListItems;

        public IList<Ingredient> ShoppingListItems
        {
            get { return shoppingListItems ?? (shoppingListItems = GetShoppingListItems().ToList()); }
        }

        public void DecreaseAmount(Ingredient item)
        {
            shoppingListItems = Ingredient.DecreaseAmount(item).Match(
                i => shoppingListItems.Replpace(item, i),
                () => shoppingListItems.Without(item).ToList()
            );
            // TODO: construct request and send API call
        }

        public void IncreaseAmount(Ingredient item)
        {
            var increasedItem = Ingredient.IncreaseAmount(item);
            var newItem = increasedItem.IfNone(() => throw new InvalidOperationException());
            shoppingListItems = shoppingListItems.Replpace(item, newItem);
            // TODO: construct request and send API call
        }

        public void Add(Foodstuff foodstuff)
        {
            var newItem = Ingredient.Create(foodstuff);
            shoppingListItems.Add(newItem);
            // TODO: construct request and send API call
        }

        public IEnumerable<Foodstuff> Search(string query)
        {
            // TODO: implement with API
            var foodstuff = new[]
            {
                new Foodstuff(
                    Guid.NewGuid(),
                    "Carrot",
                    new Uri("https://www.znaturalfoods.com/698-thickbox_default/carrot-powder-organic.jpg"),
                    new Amount(1, AmountUnit.Piece),
                    new Amount(1, AmountUnit.Piece)
                ),
                new Foodstuff(
                    Guid.NewGuid(),
                    "Bacon",
                    new Uri("https://upload.wikimedia.org/wikipedia/commons/3/31/Made20bacon.png"),
                    new Amount(100, AmountUnit.Gram),
                    new Amount(50, AmountUnit.Gram)
                )
            };
            return foodstuff.Except(ShoppingListItems.Select(i => i.Foodstuff));
        }

        private IEnumerable<Ingredient> GetShoppingListItems()
        {
            return apiClient.GetShoppingList().Items.Select(i =>
                Ingredient.Create(
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
