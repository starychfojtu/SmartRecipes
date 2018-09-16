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
        | FoodstuffAlreadyAdded
        
    type ShoppingListActionDao = {
        tokens: TokensDao
        shoppingLists: ShoppingsListsDao
    }
        
    let private authorize accessToken authorizeError =
        Users.authorize authorizeError accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private getShoppinglist accountId =
        Reader(fun dao -> dao.shoppingLists.get accountId |> Ok)
        
    let private updateDb list = 
        Reader(fun dao -> dao.shoppingLists.update list |> Ok)
    
    let private shoppingListAction accessToken authorizeError action =
        authorize accessToken authorizeError
        >>=! getShoppinglist
        >>=! action
        >>=! updateDb
        
    // Add foodstuff
        
    let private addFoodstuffsToList foodstuffs list = 
        ShoppingList.addFoodstuffs list foodstuffs |> Result.mapError (fun _ -> FoodstuffAlreadyAdded) |> Reader.id
        
    let addFoodstuffs accesToken foodstuffs = 
        shoppingListAction accesToken Unauthorized (addFoodstuffsToList foodstuffs)
        
    // Add recipe
        
    let private addRecipesToList recipes list = 
        ShoppingList.addRecipes list recipes |> Result.mapError (fun _ -> FoodstuffAlreadyAdded) |> Reader.id
        
    let addRecipes accessToken recipe = 
        shoppingListAction accessToken Unauthorized (addRecipesToList recipe)

    // Change amount
    
    type ChangeAmountError =
        | Unauthorized
        | ItemNotInShoppingList
    
    let private changeFoodstuffAmount foodstuff newAmount list =
        let mapError = (Result.mapError (fun _ -> ItemNotInShoppingList))
        ShoppingList.changeAmount foodstuff newAmount list |> mapError |> Reader.id
        
    let changeAmount accessToken foodstuff newAmount =
        shoppingListAction accessToken Unauthorized (changeFoodstuffAmount foodstuff newAmount)
        
    // Change person count
        
    let private changeRecipePersonCount recipe newPersonCount list =
        let mapError = (Result.mapError (fun _ -> ItemNotInShoppingList))
        ShoppingList.changePersonCount recipe newPersonCount list |> mapError |> Reader.id
        
    let changePersonCount accessToken recipe newPersonCount =
        shoppingListAction accessToken Unauthorized (changeRecipePersonCount recipe newPersonCount)
        
    // Cook recipe
    
    type CookRecipeError =
        | Unauthorized
        | RecipeNotInList
        | NotEnoughIngredientsInList
        
    let private authorizeCook accessToken =
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private toUseCaseError = function 
        | ShoppingList.CookRecipeError.RecipeNotInList -> RecipeNotInList
        | ShoppingList.CookRecipeError.NotEnoughIngredientsInList -> NotEnoughIngredientsInList
        
    let private cookRecipe recipe list =
        ShoppingList.cook recipe list |> Result.mapError toUseCaseError |> Reader.id
    
    let cook accessToken recipe = 
        authorizeCook accessToken
        >>=! getShoppinglist
        >>=! cookRecipe recipe
        