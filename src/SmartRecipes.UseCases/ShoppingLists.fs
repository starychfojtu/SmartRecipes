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
        | NotAuthorized
        | FoodstuffAlreadyAdded
        
    type AddItemDao = {
        tokens: TokensDao
        users: UsersDao
        shoppingLists: ShoppingsListsDao
    }
        
    let private authorize accessToken =
        Users.authorizeWithAccount NotAuthorized accessToken |> mapEnviroment (fun dao -> (dao.tokens, dao.users))
        
    let private getShoppinglist account =
        Reader(fun dao -> dao.shoppingLists.get account |> Ok)
        
    let private updateDb list = 
        Reader(fun dao -> dao.shoppingLists.update list |> Ok)
    
    let private shoppingListAction accessToken action =
        authorize accessToken
        >>=! getShoppinglist
        >>=! action
        >>=! updateDb
        
    let private addFoodstuffToList foodstuff list = 
        ShoppingList.addFoodstuff list foodstuff |> Result.mapError (fun _ -> FoodstuffAlreadyAdded) |> Reader.id
        
    let addFoodstuff accesToken foodstuff = 
        shoppingListAction accesToken (addFoodstuffToList foodstuff)
        
    let private addRecipeToList recipe list = 
        ShoppingList.addRecipe list recipe |> Result.mapError (fun _ -> FoodstuffAlreadyAdded) |> Reader.id
        
    let addRecipe accesToken recipe = 
        shoppingListAction accesToken (addRecipeToList recipe)
        
    