module UseCases.ShoppingLists
    open DataAccess.Recipes
    open DataAccess.ShoppingLists
    open DataAccess.Tokens
    open DataAccess.Users
    open Domain
    open Domain
    open Domain.ShoppingList
    open Domain.ShoppingList
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Reader
    
    type AddItemError =
        | Unauthorized
        | DomainError of ShoppingList.AddItemError
        
    type ShoppingListActionDao = {
        tokens: TokensDao
        shoppingLists: ShoppingsListsDao
    }
        
    let private authorize accessToken authorizeError =
        Users.authorize authorizeError accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private getShoppinglist accountId =
        Reader(fun dao -> dao.shoppingLists.get accountId |> Ok)
        
    let private updateShoppingList list = 
        Reader(fun dao -> dao.shoppingLists.update list |> Ok)
    
    let private shoppingListAction accessToken authorizeError action =
        authorize accessToken authorizeError
        >>=! getShoppinglist
        >>=! action
        >>=! updateShoppingList
        
    // Add foodstuff
        
    let private addFoodstuffsToList foodstuffs list = 
        ShoppingList.addFoodstuffs list foodstuffs |> Result.mapError DomainError |> Reader.id
        
    let addFoodstuffs accesToken foodstuffs = 
        shoppingListAction accesToken Unauthorized (addFoodstuffsToList foodstuffs)
        
    // Add recipe
        
    let private addRecipesToList recipes list = 
        ShoppingList.addRecipes list recipes |> Result.mapError DomainError |> Reader.id
        
    let addRecipes accessToken recipe = 
        shoppingListAction accessToken Unauthorized (addRecipesToList recipe)

    // Change amounts
    
    type ChangeAmountError =
        | Unauthorized
        | DomainError of ShoppingList.ChangeAmountError
    
    let private changeFoodstuffAmount foodstuff newAmount list =
        ShoppingList.changeAmount foodstuff newAmount list |> Result.mapError DomainError |> Reader.id
        
    let changeAmount accessToken foodstuff newAmount =
        shoppingListAction accessToken Unauthorized (changeFoodstuffAmount foodstuff newAmount)
        
    // Change person count
        
    let private changeRecipePersonCount recipe newPersonCount list =
        ShoppingList.changePersonCount recipe newPersonCount list |> Result.mapError DomainError |> Reader.id
        
    let changePersonCount accessToken recipe newPersonCount =
        shoppingListAction accessToken Unauthorized (changeRecipePersonCount recipe newPersonCount)
        
    // Cook recipe
    
    type CookRecipeError =
        | Unauthorized
        | DomainError of ShoppingList.CookRecipeError
        
    let private cookRecipe recipe list =
        ShoppingList.cook recipe list |> Result.mapError DomainError |> Reader.id
    
    let cook accessToken recipe = 
        shoppingListAction accessToken Unauthorized (cookRecipe recipe)
        
    // Remove foodstuff 
    
    type RemoveItemError =
        | Unauthorized
        | DomainError of ShoppingList.RemoveItemError
    
    let private removeFoodstuffItem foodstuff list =
        removeFoodstuff list foodstuff |> Result.mapError DomainError |> Reader.id
    
    let removeFoodstuff accessToken foodstuff = 
        shoppingListAction accessToken Unauthorized (removeFoodstuffItem foodstuff)
        
    // Remove recipe
    
    let private removeRecipeItem foodstuff list =
        removeRecipe list foodstuff |> Result.mapError DomainError |> Reader.id
    
    let removeRecipe accessToken foodstuff = 
        shoppingListAction accessToken Unauthorized (removeRecipeItem foodstuff)