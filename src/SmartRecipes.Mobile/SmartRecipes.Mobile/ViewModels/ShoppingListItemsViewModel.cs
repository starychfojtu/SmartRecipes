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
        private readonly DataAccess dataAccess;

        private IImmutableList<ShoppingListItem> shoppingListItems { get; set; }

        private IImmutableDictionary<IFoodstuff, IAmount> requiredAmounts { get; set; }

        public ShoppingListItemsViewModel(DataAccess dataAccess)
        {
            shoppingListItems = ImmutableList.Create<ShoppingListItem>();
            this.dataAccess = dataAccess;
        }

        public IEnumerable<FoodstuffAmountCellViewModel> ShoppingListItems
        {
            get { return shoppingListItems.Select(i => ToViewModel(i)); }
        }

        public override async Task InitializeAsync()
        {
            requiredAmounts = await ShoppingListRepository.GetRequiredAmounts(CurrentAccount)(dataAccess);
            UpdateShoppingListItems(await ShoppingListRepository.GetItems(CurrentAccount)(dataAccess));
        }

        public async Task Refresh()
        {
            await InitializeAsync();
        }

        public async Task OpenAddFoodstuffDialog()
        {
            var selected = await Navigation.SelectFoodstuffDialog();
            var newShoppingListItems = await ShoppingListHandler.AddToShoppingList(dataAccess, CurrentAccount, selected);
            var allShoppingListItems = shoppingListItems.Concat(newShoppingListItems);
            UpdateShoppingListItems(allShoppingListItems);
        }

        private async Task ShoppingListItemAction(ShoppingListItem shoppingListItem, Func<IShoppingListItemAmount, IFoodstuff, IShoppingListItemAmount> action)
        {
            var newAmount = action(shoppingListItem.ItemAmount, shoppingListItem.Foodstuff);
            var newShoppingListItem = shoppingListItem.WithItemAmount(newAmount);

            var oldItem = shoppingListItems.First(i => i.Foodstuff.Id == shoppingListItem.Foodstuff.Id);
            var newShoppingListItems = CollectionExtensions.Replace(shoppingListItems, oldItem, newShoppingListItem);

            await ShoppingListHandler.Update(dataAccess, newShoppingListItem.ItemAmount.ToEnumerable().ToImmutableList());
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
                () => ShoppingListItemAction(shoppingListItem, (ia, f) => ShoppingListHandler.Increase(ia, f)),
                () => ShoppingListItemAction(shoppingListItem, (ia, f) => ShoppingListHandler.Decrease(ia, f))
            );
        }
    }
}
