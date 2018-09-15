module Api.ShoppingLists
    open Api
    open DataAccess
    open DataAccess.Foodstuffs
    open DataAccess.Recipes
    open DataAccess.ShoppingLists
    open DataAccess.Tokens
    open Domain
    open Domain.NaturalNumber
    open Domain.NonNegativeFloat
    open FSharpPlus
    open FSharpPlus
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Reader
    open Infrastructure.Validation
    open System
    open UseCases
    open UseCases.ShoppingLists
    open Generic
    open FSharpPlus.Data.Validation
    
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
        
    let getShoppingListActionDao () = {
        tokens = Tokens.getDao ()
        shoppingLists = ShoppingLists.getDao ()
    }

    // Add foodstuffs
    
    let getAddFoodstuffDao () = (getShoppingListActionDao (), Foodstuffs.getDao().getByIds)
    
    let addFoodstuffs accessToken parameters =
        addItems ShoppingLists.addFoodstuffs accessToken parameters
        
    let addFoodstuffsHandler ctx next =
        authorizedPostHandler (getAddFoodstuffDao ()) ctx next addFoodstuffs
        
    // Add recipes
    
    let getAddRecipesDao () = (getShoppingListActionDao (), Recipes.getDao().getByIds)
    
    let addRecipes accessToken parameters =
        addItems ShoppingLists.addRecipes accessToken parameters
        
    let addRecipesHandler ctx next =
        authorizedPostHandler (getAddRecipesDao ()) ctx next addRecipes
        
    // Change foodstuff amount
    
    type ChangeAmountParameters = {
        foodstuffId: Guid
        amount: float
    }
    
    type ChangeAmountError = 
        | FoodstuffNotFound
        | AmountMustBePositive
        | BusinessError of UseCases.ShoppingLists.ChangeAmountError
        
    type ChangeAmountDao = {
        shoppingListAction: ShoppingListActionDao
        foodstuffs: FoodstuffDao
    }
    
    let private getChangeAmountDao () = {
        shoppingListAction = getShoppingListActionDao ()
        foodstuffs = Foodstuffs.getDao ()
    }
        
    let private mkFoodstuff id =
        Reader(fun dao -> dao.foodstuffs.getById id |> Option.toResult [FoodstuffNotFound])
        
    let private mkAmount amount foodstuff = 
        mkNonNegativeFloat amount |> mapFailure (fun _ -> [AmountMustBePositive]) |> map (fun a -> (a, foodstuff)) |> toResult |> Reader.id
        
    let private change accessToken foodstuff amount =
        changeAmount accessToken foodstuff amount
        |> Reader.mapEnviroment (fun dao -> dao.shoppingListAction)
        |> (Reader.map (Result.mapError (fun e -> [BusinessError(e)])))
        
    let changeAmount accessToken parameters =
        mkFoodstuff parameters.foodstuffId
        >>=! mkAmount parameters.amount
        >>=! (fun (newAmount, foodstuff) -> change accessToken foodstuff newAmount)
        
    let changeAmountHandler ctx next =
        authorizedPostHandler (getChangeAmountDao ()) ctx next changeAmount
        
    // Chnage person count