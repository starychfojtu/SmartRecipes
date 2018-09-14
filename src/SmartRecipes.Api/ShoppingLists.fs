module Api.ShoppingLists
    open Api
    open DataAccess
    open DataAccess
    open DataAccess.Foodstuffs
    open DataAccess.ShoppingLists
    open DataAccess.Tokens
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Reader
    open System
    open UseCases
    open UseCases.ShoppingLists
    open Generic

    type AddFoodstuffsParameters = {
        foodstuffIds: seq<Guid>
    }
    
    type AddFoodstuffsError =
        | FoodstuffNotFound
        | BusinessError of AddItemError
        
    type AddFoodstuffsDao = {
        tokens: TokensDao
        shoppingLists: ShoppingsListsDao
        foodstuffs: FoodstuffDao
    }
    
    let getAddFoodstuffDao () = {
        tokens = Tokens.getDao ()
        shoppingLists = ShoppingLists.getDao ()
        foodstuffs = Foodstuffs.getDao ()
    }
        
    let getFoodstuffs parameters = 
        Reader(fun dao -> 
            let foodstuffs = dao.foodstuffs.getByIds parameters.foodstuffIds
            let foundAllFoodstuffs = Seq.length foodstuffs = Seq.length parameters.foodstuffIds
            if foundAllFoodstuffs
                then Ok foodstuffs
                else Error FoodstuffNotFound
        )
        
    let private addToShoppingList accesstToken foodstuffs = 
        ShoppingLists.addFoodstuffs accesstToken foodstuffs
        |> Reader.map (Result.mapError (fun e -> BusinessError e))
        |> Reader.mapEnviroment (fun dao -> { tokens = dao.tokens; shoppingLists = dao.shoppingLists })

    let addFoodstuffs accessToken parameters = 
        getFoodstuffs parameters >>=! addToShoppingList accessToken
        
    let addFoodstuffsHandler ctx next =
        authorizedPostHandler (getAddFoodstuffDao ()) ctx next addFoodstuffs