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
using LanguageExt.SomeHelp;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly ApiClient apiClient;

        private readonly Database database;

        private IImmutableList<ShoppingListItem> shoppingListItems { get; set; }

        public ShoppingListItemsViewModel(ApiClient apiClient, Database database)
        {
            this.apiClient = apiClient;
            this.database = database;
            shoppingListItems = ImmutableList.Create<ShoppingListItem>();
        }

        public IEnumerable<FoodstuffAmountCellViewModel> ShoppingListItems
        {
            get { return shoppingListItems.Select(i => ToViewModel(i)); }
        }

        public override async Task InitializeAsync()
        {
            await UpdateItemsAsync();
        }

        public async Task Refresh()
        {
            await InitializeAsync();
        }

        public async Task OpenAddFoodstuffDialog()
        {
            var selected = await Navigation.SelectFoodstuffDialog();
            var newShoppingListItems = await ShoppingListHandler.AddToShoppingList(apiClient, database, CurrentAccount.ToSome(), selected);
            var allShoppingListItems = shoppingListItems.Concat(newShoppingListItems).ToImmutableList();
            UpdateShoppingListItems(allShoppingListItems);
        }

        private async Task ShoppingListItemAction(ShoppingListItem shoppingListItem, Func<IShoppingListItemAmount, IFoodstuff, IShoppingListItemAmount> action)
        {
            var newAmount = action(shoppingListItem.ItemAmount, shoppingListItem.Foodstuff);
            var newShoppingListItem = shoppingListItem.WithItemAmount(newAmount.ToSome());

            var oldItem = shoppingListItems.First(i => i.Foodstuff.Id == shoppingListItem.Foodstuff.Id);
            var newShoppingListItems = shoppingListItems.Replace(oldItem, newShoppingListItem);

            await ShoppingListHandler.Update(apiClient, database, newShoppingListItem.ItemAmount.ToEnumerable());
            UpdateShoppingListItems(newShoppingListItems);
        }

        private async Task UpdateItemsAsync()
        {
            UpdateShoppingListItems((await ShoppingListRepository.GetItems(apiClient, database, CurrentAccount.ToSome())));
        }

        private void UpdateShoppingListItems(IEnumerable<ShoppingListItem> newShoppingListItems)
        {
            shoppingListItems = newShoppingListItems.OrderBy(i => i.Foodstuff.Name).ToImmutableList();
            RaisePropertyChanged(nameof(ShoppingListItems));
        }

        private FoodstuffAmountCellViewModel ToViewModel(ShoppingListItem shoppingListItem)
        {
            return new FoodstuffAmountCellViewModel(
                shoppingListItem.Foodstuff.ToSome(),
                shoppingListItem.Amount.ToSome(),
                () => ShoppingListItemAction(shoppingListItem, (ia, f) => ShoppingListHandler.Increase(ia, f)),
                () => ShoppingListItemAction(shoppingListItem, (ia, f) => ShoppingListHandler.Decrease(ia, f))
            );
        }
    }
}
