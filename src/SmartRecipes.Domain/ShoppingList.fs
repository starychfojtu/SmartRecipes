module Domain.ShoppingList
    open Domain
    open Domain.Account
    open Domain.Foodstuff
    open Domain.NonNegativeFloat
    open Domain.Recipe
    open System
    
    type ShoppingListId = ShoppingListId of Guid
    
    type ShoppingList = {
        id: ShoppingListId
        accountId: AccountId
    }

    let createShoppingList accountId = {
        id = ShoppingListId(Guid.NewGuid ())
        accountId = accountId
    }
    
    type ShoppingListItem = {
        listId: ShoppingListId
        foodstuffId: FoodstuffId
        amount: NonNegativeFloat
    }
    
    let createShoppingListItem listId foodstuffId amount = {
        listId = listId
        foodstuffId = foodstuffId
        amount = amount
    }
    
    type ShoppingListRecipeItem = {
        listId: ShoppingListId
        recipeId: RecipeId
        personCount: NonNegativeFloat
    }
    
    let createShoppingListRecipeItem listId recipeId personCount = {
        listId = listId
        recipeId = recipeId
        personCount = personCount
    }