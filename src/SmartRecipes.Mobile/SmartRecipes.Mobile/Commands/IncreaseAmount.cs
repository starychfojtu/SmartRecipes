using System;

namespace SmartRecipes.Mobile.Commands
{
    public class IncreaseAmount
    {
        public IncreaseAmount(Ingredient ingredient)
        {
            Ingredient = ingredient;
        }

        public Ingredient Ingredient { get; }
    }
}
