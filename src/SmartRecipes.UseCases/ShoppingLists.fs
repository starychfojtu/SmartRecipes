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
        ShoppingList.addFoodstuffs list foodstuffs |> Result.mapError DomainError |> ReaderT.id
        
    let addFoodstuffs accesToken foodstuffs = 
        shoppingListAction accesToken Unauthorized (addFoodstuffsToList foodstuffs)
        
    // Add recipe
        
    let private addRecipesToList recipes list = 
        ShoppingList.addRecipes list recipes |> Result.mapError DomainError |> ReaderT.id
        
    let addRecipes accessToken recipe = 
        shoppingListAction accessToken Unauthorized (addRecipesToList recipe)

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
    
    type RemoveItemError =
        | Unauthorized
        | DomainError of ShoppingList.RemoveItemError
    
    let private removeFoodstuffItem foodstuffId list =
        removeFoodstuff foodstuffId list |> Result.mapError DomainError |> ReaderT.id
    
    let removeFoodstuff accessToken foodstuffId = 
        shoppingListAction accessToken Unauthorized (removeFoodstuffItem foodstuffId)
        
    // Remove recipe
    
    let private removeRecipeItem recipe list =
        removeRecipe list recipe |> Result.mapError DomainError |> ReaderT.id
    
    let removeRecipe accessToken foodstuff = 
        shoppingListAction accessToken Unauthorized (removeRecipeItem foodstuff)