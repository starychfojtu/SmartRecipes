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
            Ingredients = new List<Ingredient>();
        }

        public IEnumerable<FoodstuffCellViewModel> Items
        {
            get { return Ingredients.Select(i => ToViewModel(i)); }
        }

        private IList<Ingredient> Ingredients { get; set; }

        public override async Task InitializeAsync()
        {
            await UpdateItemsAsync();
        }

        public void Refresh()
        {
            RaisePropertyChanged(nameof(Items));
        }

        public async Task AddItem()
        {
            await Navigation.AddShoppingListItem(this);
        }

        private async Task IncreaseAmountAsync(Ingredient ingredient)
        {
            var increased = await commandHandler.IncreaseAmount(ingredient);
            UpdateItems(Ingredients.Replace(ingredient, increased));
        }

        private async Task DecreaseAmountAsync(Ingredient ingredient)
        {
            var decreased = await commandHandler.DecreaseAmount(ingredient);
            UpdateItems(Ingredients.Replace(ingredient, decreased));
        }

        private async Task UpdateItemsAsync()
        {
            UpdateItems((await repository.GetItems()).ToList());
        }

        private void UpdateItems(IList<Ingredient> ingredients)
        {
            Ingredients = ingredients;
            RaisePropertyChanged(nameof(Items));
        }

        private FoodstuffCellViewModel ToViewModel(Ingredient ingredient)
        {
            return new FoodstuffCellViewModel(
                ingredient.Foodstuff,
                ingredient.Amount,
                () => IncreaseAmountAsync(ingredient),
                () => DecreaseAmountAsync(ingredient)
            );
        }
    }
}
