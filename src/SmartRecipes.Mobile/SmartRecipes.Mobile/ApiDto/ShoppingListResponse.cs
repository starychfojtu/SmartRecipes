using System;
using System.Collections.Generic;

namespace SmartRecipes.Mobile.ApiDto
{
    public class ShoppingListResponse
    {
        public ShoppingListResponse(IEnumerable<Item> items)
        {
            Items = items;
        }

        public IEnumerable<Item> Items { get; }

        public class Item
        {
            public Item(Foodstuff foodstuff, Amount amount)
            {
                FoodstuffDto = foodstuff;
                Amount = amount;
            }

            public Foodstuff FoodstuffDto { get; }

            public Amount Amount { get; }

            public class Foodstuff
            {
                public Foodstuff(Guid id, string name, Uri imageUrl, Amount baseAmount, Amount amountStep)
                {
                    Id = id;
                    Name = name;
                    ImageUrl = imageUrl;
                    BaseAmount = baseAmount;
                    AmountStep = amountStep;
                }

                public Guid Id { get; }

                public string Name { get; }

                public Uri ImageUrl { get; }

                public Amount BaseAmount { get; }

                public Amount AmountStep { get; }
            }
        }
    }
}
