using LanguageExt;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItem
    {
        public ShoppingListItem(Some<Foodstuff> foodstuff, Some<Ingredient> ingredient)
        {
            Foodstuff = foodstuff;
            Ingredient = ingredient;
        }

        public Foodstuff Foodstuff { get; }

        public Ingredient Ingredient { get; }

        public Amount Amount
        {
            get { return Ingredient.Amount; }
        }

        public ShoppingListItem WithIngredient(Some<Ingredient> ingredient)
        {
            return new ShoppingListItem(Foodstuff, ingredient);
        }
    }
}
