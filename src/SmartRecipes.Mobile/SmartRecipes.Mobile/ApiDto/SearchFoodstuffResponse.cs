using System;
using System.Collections;
using System.Collections.Generic;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ApiDto
{
    public class SearchFoodstuffResponse
    {
        public SearchFoodstuffResponse(IEnumerable<Foodstuff> foodstuffs)
        {
            Foodstuffs = foodstuffs;
        }

        public IEnumerable<Foodstuff> Foodstuffs { get; }

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
