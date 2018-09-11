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
        users: UsersDao
        shoppingLists: ShoppingsListsDao
    }
        
    let private authorize accessToken authorizeError =
        Users.authorizeWithAccount authorizeError accessToken |> mapEnviroment (fun dao -> (dao.tokens, dao.users))
        
    let private getShoppinglist account =
        Reader(fun dao -> dao.shoppingLists.get account |> Ok)
        
    let private updateDb list = 
        Reader(fun dao -> dao.shoppingLists.update list |> Ok)
    
    let private shoppingListAction accessToken authorizeError action =
        authorize accessToken authorizeError
        >>=! getShoppinglist
        >>=! action
        >>=! updateDb
        
    // Add foodstuff
        
    let private addFoodstuffToList foodstuff list = 
        ShoppingList.addFoodstuff list foodstuff None |> Result.mapError (fun _ -> FoodstuffAlreadyAdded) |> Reader.id
        
    let addFoodstuff accesToken foodstuff = 
        shoppingListAction accesToken Unauthorized (addFoodstuffToList foodstuff)
        
    // Add recipe
        
    let private addRecipeToList recipe list = 
        ShoppingList.addRecipe list recipe None |> Result.mapError (fun _ -> FoodstuffAlreadyAdded) |> Reader.id
        
    let addRecipe accessToken recipe = 
        shoppingListAction accessToken Unauthorized (addRecipeToList recipe)
        
    type ChangeAmountError =
        | Unauthorized
        | ItemNotFound
        
    // Change amount
        
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