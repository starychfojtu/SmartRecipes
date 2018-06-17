using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using System;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;
using System.Collections.Immutable;
using SmartRecipes.Mobile.Models;
using LanguageExt;
using SmartRecipes.Mobile.Extensions;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly Enviroment enviroment;

        private IImmutableList<ShoppingListItem> shoppingListItems { get; set; }

        private IImmutableDictionary<IFoodstuff, IAmount> requiredAmounts { get; set; }

        public ShoppingListItemsViewModel(Enviroment enviroment)
        {
            shoppingListItems = ImmutableList.Create<ShoppingListItem>();
            this.enviroment = enviroment;
        }

        public IEnumerable<FoodstuffAmountCellViewModel> ShoppingListItems
        {
            get { return shoppingListItems.Select(i => ToViewModel(i)); }
        }

        public override async Task InitializeAsync()
        {
            requiredAmounts = await ShoppingListRepository.GetRequiredAmounts(CurrentAccount)(enviroment);
            UpdateShoppingListItems(await ShoppingListRepository.GetItems(CurrentAccount)(enviroment));
        }

        public async Task Refresh()
        {
            await InitializeAsync();
        }

        public async Task OpenAddFoodstuffDialog()
        {
            var selected = await Navigation.SelectFoodstuffDialog();
            var newShoppingListItems = await ShoppingListHandler.AddToShoppingList(enviroment, CurrentAccount, selected);
            var allShoppingListItems = shoppingListItems.Concat(newShoppingListItems);
            UpdateShoppingListItems(allShoppingListItems);
        }

        private async Task ShoppingListItemAction(ShoppingListItem shoppingListItem, Func<ShoppingListItem, IShoppingListItemAmount> action)
        {
            var newAmount = action(shoppingListItem);
            var newShoppingListItem = shoppingListItem.WithItemAmount(newAmount);

            var oldItem = shoppingListItems.First(i => i.Foodstuff.Id == shoppingListItem.Foodstuff.Id);
            var newShoppingListItems = CollectionExtensions.Replace(shoppingListItems, oldItem, newShoppingListItem);

            await ShoppingListHandler.Update(enviroment, newShoppingListItem.ItemAmount.ToEnumerable().ToImmutableList());
            UpdateShoppingListItems(newShoppingListItems);
        }

        private void UpdateShoppingListItems(IEnumerable<ShoppingListItem> newShoppingListItems)
        {
            shoppingListItems = newShoppingListItems.OrderBy(i => i.Foodstuff.Name).ToImmutableList();
            RaisePropertyChanged(nameof(ShoppingListItems));
        }

        private FoodstuffAmountCellViewModel ToViewModel(ShoppingListItem shoppingListItem)
        {
            return new FoodstuffAmountCellViewModel(
                shoppingListItem.Foodstuff,
                shoppingListItem.Amount,
                requiredAmounts.TryGetValue(shoppingListItem.Foodstuff),
                () => ShoppingListItemAction(shoppingListItem, i => ShoppingListHandler.Increase(i)),
                () => ShoppingListItemAction(shoppingListItem, i => ShoppingListHandler.Decrease(i))
            );
        }
    }
}
