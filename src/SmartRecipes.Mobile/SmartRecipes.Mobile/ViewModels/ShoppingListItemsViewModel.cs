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

        public IEnumerable<FoodstuffCellViewModel> Items { get; private set; }

        public override async Task InitializeAsync()
        {
            await UpdateItemsAsync();
        }

        public void Refresh()
        {
            RaisePropertyChanged(nameof(Items));
        }

        public void AddItem()
        {
            Navigation.AddShoppingListItem(this);
        }

        private async Task UpdateItemsAsync()
        {
            UpdateItems(await repository.GetItems());
        }

        private void UpdateItems(IEnumerable<Ingredient> ingredients)
        {
            Items = ingredients.Select(i => ToViewModel(i));
            RaisePropertyChanged(nameof(Items));
        }

        private async Task IncreaseAmountAsync(Ingredient item)
        {
            UpdateItems(await commandHandler.Handle(new IncreaseAmount(item)));
        }

        private async Task DecreaseAmountAsync(Ingredient item)
        {
            UpdateItems(await commandHandler.Handle(new DecreaseAmount(item)));
        }

        private static FoodstuffCellViewModel ToViewModel(Ingredient ingredient)
        {
            return new FoodstuffCellViewModel(
                ingredient.Foodstuff,
                ingredient.Amount,
                () => { }, //TODO: IncreaseItemAmount(ingredient),
                () => { } //TODO: DecreaseItemAmount(ingredient)
            );
        }
    }
}
