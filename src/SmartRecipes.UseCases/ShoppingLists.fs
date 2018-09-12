module UseCases.ShoppingLists
    open DataAccess.ShoppingLists
    open DataAccess.Tokens
    open DataAccess.Users
    open Domain
    open Domain
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Reader
    
    type AddItemError =
        | Unauthorized
        | FoodstuffAlreadyAdded
        
    type AddItemDao = {
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
        | ItemNotFound
    
    let private changeFoodstuffAmount foodstuff newAmount list =
        let mapError = (Result.mapError (fun _ -> ItemNotFound))
        ShoppingList.changeAmount foodstuff newAmount list |> mapError |> Reader.id
        
    let changeAmount accessToken foodstuff newAmount =
        shoppingListAction accessToken Unauthorized (changeFoodstuffAmount foodstuff newAmount)
        
    // Change person count
        
    let private changeRecipePersonCount recipe newPersonCount list =
        let mapError = (Result.mapError (fun _ -> ItemNotFound))
        ShoppingList.changePersonCount recipe newPersonCount list |> mapError |> Reader.id
        
    let changePersonCount accessToken recipe newPersonCount =
        shoppingListAction accessToken Unauthorized (changeFoodstuffAmount recipe newPersonCount)