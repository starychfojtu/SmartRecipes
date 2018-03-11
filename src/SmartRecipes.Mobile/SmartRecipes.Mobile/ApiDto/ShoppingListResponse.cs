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
            public Item(Foodstuff foodstuff, int amount)
            {
                FoodstuffDto = foodstuff;
                Amount = amount;
            }

            public Foodstuff FoodstuffDto { get; }

            public int Amount { get; }

            public class Foodstuff
            {
                public Foodstuff(Guid id, string name, string imageUrl)
                {
                    Id = id;
                    Name = name;
                    ImageUrl = imageUrl;
                }

                public Guid Id { get; }

                public string Name { get; }

                public string ImageUrl { get; }
            }
        }
    }
}
