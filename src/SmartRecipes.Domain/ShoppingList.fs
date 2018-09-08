module Domain.ShoppingList
    open Domain.Account
    open Domain.Foodstuff
    open Domain.NonNegativeFloat
    
    type ShoppingList = {
        accountId: AccountId
    }
    
    let createShoppingList accountId = {
        accountId = accountId
    }
    
    type ShoppingListItem = {
        list: ShoppingList
        foodstuff: Foodstuff
        amount: NonNegativeFloat
    }