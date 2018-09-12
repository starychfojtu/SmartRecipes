module Api.ShoppingLists
    open Api
    open DataAccess.Foodstuffs
    open DataAccess.ShoppingLists
    open DataAccess.Tokens
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Reader
    open System
    open UseCases
    open UseCases.ShoppingLists

    type AddFoodstuffParameters = {
        foodstuffIds: seq<Guid>
    }
    
    type AddFoodstuffError =
        | FoodstuffNotFound
        | BusinessError of AddItemError
        
    type AddFoodstuffDao = {
        tokens: TokensDao
        shoppingLists: ShoppingsListsDao
        foodstuffs: FoodstuffDao
    }
        
    let getFoodstuffs parameters = 
        Reader(fun dao -> 
            let foodstuffs = dao.foodstuffs.getByIds parameters.foodstuffIds
            if Seq.length foodstuffs = Seq.length parameters.foodstuffIds
                then Ok foodstuffs
                else Error FoodstuffNotFound
        )
        
    let addToShoppingList accesstToken foodstuffs = 
        ShoppingLists.addFoodstuffs accesstToken foodstuffs
        |> Reader.map (Result.mapError (fun e -> BusinessError e))
        |> Reader.mapEnviroment (fun (dao: AddFoodstuffDao) -> { tokens = dao.tokens; shoppingLists = dao.shoppingLists })

    let addFoodstuff accessToken parameters = 
        getFoodstuffs parameters >>=! addToShoppingList accessToken