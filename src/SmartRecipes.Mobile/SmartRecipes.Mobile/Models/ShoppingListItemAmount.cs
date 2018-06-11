using System;
using LanguageExt;

namespace SmartRecipes.Mobile.Models
{
    public class ShoppingListItemAmount : FoodstuffAmount, IShoppingListItemAmount
    {
        private ShoppingListItemAmount(Guid id, Guid shoppingListOwnerId, Guid foodstuffId, IAmount amount) : base(id, foodstuffId, amount)
        {
            ShoppingListOwnerId = shoppingListOwnerId;
        }

        public ShoppingListItemAmount() { /* SQLite */ }

        public Guid ShoppingListOwnerId { get; set; }

        public IShoppingListItemAmount WithAmount(IAmount amount)
        {
            return new ShoppingListItemAmount(Id, ShoppingListOwnerId, FoodstuffId, amount);
        }

        public static IShoppingListItemAmount Create(Some<IAccount> owner, Some<IFoodstuff> foodstuff, IAmount amount)
        {
            return new ShoppingListItemAmount(Guid.NewGuid(), owner.Value.Id, foodstuff.Value.Id, amount);
        }

        public static IShoppingListItemAmount Create(Guid id, Guid shoppingListOwnerId, Guid foodstuffId, IAmount amount)
        {
            return new ShoppingListItemAmount(id, shoppingListOwnerId, foodstuffId, amount);
        }
    }
}
