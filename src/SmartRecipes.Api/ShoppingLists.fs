module Api.ShoppingLists
    open Api
    open Api
    open DataAccess
    open DataAccess
    open DataAccess.Foodstuffs
    open DataAccess.Recipes
    open DataAccess.ShoppingLists
    open DataAccess.Tokens
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Reader
    open System
    open UseCases
    open UseCases.ShoppingLists
    open Generic
    
    // Add
    
    type AddItemsParameters = {
        itemIds: seq<Guid>
    }
    
    type AddItemsError =
        | InvalidIds
        | BusinessError of AddItemError
        
    let getItems parameters = 
        Reader(fun (dao, getByIds) -> 
            let itemIds = parameters.itemIds
            let items = getByIds itemIds
            let foundAll = Seq.length items = Seq.length itemIds
            if foundAll
                then Ok items
                else Error InvalidIds
        )
        
    let private addItemsToShoppingList accesstToken action items = 
        action accesstToken items
        |> Reader.map (Result.mapError (fun e -> BusinessError e))
        |> Reader.mapEnviroment (fun (dao, getByIds) -> dao)

    let addItems action accessToken parameters = 
        getItems parameters >>=! addItemsToShoppingList accessToken action
        
    let getShoppingListAction () = {
        tokens = Tokens.getDao ()
        shoppingLists = ShoppingLists.getDao ()
    }

    // Add foodstuffs
    
    let getAddFoodstuffDao () = (getShoppingListAction (), Foodstuffs.getDao().getByIds)
    
    let addFoodstuffs accessToken parameters =
        addItems ShoppingLists.addFoodstuffs accessToken parameters
        
    let addFoodstuffsHandler ctx next =
        authorizedPostHandler (getAddFoodstuffDao ()) ctx next addFoodstuffs
        
    // Add recipes
    
    let getAddRecipesDao () = (getShoppingListAction (), Recipes.getDao().getByIds)
    
    let addRecipes accessToken parameters =
        addItems ShoppingLists.addRecipes accessToken parameters
        
    let addRecipesHandler ctx next =
        authorizedPostHandler (getAddRecipesDao ()) ctx next addRecipes
        
    // Change foodstuff amount 
    
    // Change person count