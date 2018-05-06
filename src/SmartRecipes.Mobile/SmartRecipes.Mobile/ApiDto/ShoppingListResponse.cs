using System;
using System.Collections.Generic;
using SmartRecipes.Mobile.Models;

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
            public Item(Guid id, Foodstuff foodstuff, Amount amount)
            {
                Id = id;
                FoodstuffDto = foodstuff;
                Amount = amount;
            }

            public Foodstuff FoodstuffDto { get; }

            public Amount Amount { get; }

            public Guid Id { get; }

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
