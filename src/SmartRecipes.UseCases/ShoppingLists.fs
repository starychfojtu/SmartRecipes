namespace SmartRecipes.UseCases

module ShoppingLists =
    open SmartRecipes.Domain
    open SmartRecipes.Domain.ShoppingList
    open FSharpPlus.Data
    open Environment
    open Infrastructure
    
    type AddItemError =
        | Unauthorized
        | DomainError of ShoppingList.AddItemError
        
    let private getShoppinglist accountId =
        ReaderT(fun env -> env.IO.ShoppingLists.get accountId |> Ok)
        
    let private updateShoppingList list = 
        ReaderT(fun env -> env.IO.ShoppingLists.update list |> Ok)
    
    let private shoppingListAction accessToken authorizeError action =
        Users.authorize authorizeError accessToken
        >>= getShoppinglist
        >>= action
        >>= updateShoppingList
        
    // Add foodstuff
        
    let private addFoodstuffsToList foodstuffs list = 
        ShoppingList.addFoodstuffs foodstuffs list |> Result.mapError DomainError |> ReaderT.id
        
    let addFoodstuffs accesToken foodstuffs = 
        shoppingListAction accesToken Unauthorized (addFoodstuffsToList foodstuffs)
        
    // Add recipe
        
    let private addRecipesToList recipes list = 
        ShoppingList.addRecipes recipes list |> Result.mapError DomainError |> ReaderT.id
        
    let addRecipes accessToken recipes = 
        shoppingListAction accessToken Unauthorized (addRecipesToList recipes)

    // Change amounts
    
    type ChangeAmountError =
        | Unauthorized
        | DomainError of ShoppingList.ChangeAmountError
    
    let private changeFoodstuffAmount foodstuff newAmount list =
        ShoppingList.changeAmount foodstuff newAmount list |> Result.mapError DomainError |> ReaderT.id
        
    let changeAmount accessToken foodstuff newAmount =
        shoppingListAction accessToken Unauthorized (changeFoodstuffAmount foodstuff newAmount)
        
    // Change person count
        
    let private changeRecipePersonCount recipe newPersonCount list =
        ShoppingList.changePersonCount recipe newPersonCount list |> Result.mapError DomainError |> ReaderT.id
        
    let changePersonCount accessToken recipe newPersonCount =
        shoppingListAction accessToken Unauthorized (changeRecipePersonCount recipe newPersonCount)
        
    // Remove foodstuff 
    
    type RemoveItemsError =
        | Unauthorized
        | DomainError of ShoppingList.RemoveItemError
    
    let private removeFoodstuffsFromList foodstuffIds list =
        removeFoodstuffs foodstuffIds list |> Result.mapError DomainError |> ReaderT.id
    
    let removeFoodstuffs accessToken foodstuffIds = 
        shoppingListAction accessToken Unauthorized (removeFoodstuffsFromList foodstuffIds)
        
    // Remove recipe
    
    let private removeRecipesFromList recipeIds list =
        removeRecipes recipeIds list |> Result.mapError DomainError |> ReaderT.id
    
    let removeRecipes accessToken recipeIds = 
        shoppingListAction accessToken Unauthorized (removeRecipesFromList recipeIds)