using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Controllers;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.Commands;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly ShoppingListHandler commandHandler;

        private readonly ShoppingListRepository repository;

        public ShoppingListItemsViewModel(ShoppingListHandler commandHandler, ShoppingListRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
            Items = Enumerable.Empty<FoodstuffCellViewModel>();
        }

        // TODO: implement async binding
        public IEnumerable<FoodstuffCellViewModel> Items { get; private set; }

        private async Task IncreaseItemAmount(Ingredient item)
        {
            UpdateItems(await commandHandler.Handle(new IncreaseAmount(item)));
        }

        private async Task DecreaseItemAmount(Ingredient item)
        {
            UpdateItems(await commandHandler.Handle(new DecreaseAmount(item)));
        }

        public void Refresh()
        {
            RaisePropertyChanged(nameof(Items));
        }

        public void NavigateToAddItemPage()
        {
            Navigation.AddShoppingListItem(this);
        }

        private async Task UpdateItems()
        {
            UpdateItems(await repository.GetItems());
        }

        private void UpdateItems(IEnumerable<Ingredient> ingredients)
        {
            Items = ingredients.Select(i => ToViewModel(i));
            RaisePropertyChanged(nameof(Items));
        }

        private static FoodstuffCellViewModel ToViewModel(Ingredient ingredient)
        {
            // TODO: fix
            return new FoodstuffCellViewModel(
                ingredient.Foodstuff,
                ingredient.Amount,
                IncreaseItemAmount(ingredient),
                DecreaseItemAmount(item)
            );
        }
    }
}
