namespace SmartRecipes.Mobile.Commands
{
    public class DecreaseAmount
    {
        public DecreaseAmount(Ingredient ingredient)
        {
            Ingredient = ingredient;
        }

        public Ingredient Ingredient { get; }
    }
}
