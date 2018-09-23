module Api.ShoppingLists
    open Api
    open Dto
    open DataAccess
    open DataAccess.Foodstuffs
    open DataAccess.Recipes
    open DataAccess.ShoppingLists
    open DataAccess.ShoppingLists
    open DataAccess.Tokens
    open Domain
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
    open UseCases
    open UseCases
    open UseCases
    open UseCases.ShoppingLists
    
    let getShoppingListActionDao () = {
        tokens = Tokens.getDao ()
        shoppingLists = ShoppingLists.getDao ()
    }
        
    // Get 
    
    type GetShoppingListError =
        | Unauthorized
        
    let private authorize accessToken = 
        Users.authorize Unauthorized accessToken |> mapEnviroment (fun dao -> dao.tokens)
        
    let private getShoppingList accountId =
        Reader(fun dao -> dao.shoppingLists.get accountId |> Ok)
        
    let private serializeGet = 
        Result.map serializeShoppingList >> Result.mapError (function Unauthorized -> "Unaturhorized.")
    
    let get accessToken _ =
        authorize accessToken
        >>=! getShoppingList
        
    let getHandler ctx next = 
        authorizedGetHandler (getShoppingListActionDao ()) ctx next get serializeGet
    
    // Add
    
    [<CLIMutable>]
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
        
    let private serializeAddItemsError = function
        | InvalidIds -> "Invalid ids."
        | BusinessError e ->
            match e with
            | ShoppingLists.AddItemError.Unauthorized -> "Unauthorized."
            | ShoppingLists.AddItemError.DomainError de -> 
                match de with 
                | ItemAlreadyAdded -> "Item already added."
                
    let private serializeAddItems = 
        Result.map serializeShoppingList >> Result.mapError serializeAddItemsError

    let addItems action accessToken parameters = 
        getItems parameters >>=! addItemsToShoppingList accessToken action  
    

    // Add foodstuffs
    
    let private getAddFoodstuffDao () = (getShoppingListActionDao (), Foodstuffs.getDao().getByIds)
    
    let addFoodstuffs accessToken parameters =
        addItems ShoppingLists.addFoodstuffs accessToken parameters
        
    let addFoodstuffsHandler ctx next =
        authorizedPostHandler (getAddFoodstuffDao ()) ctx next addFoodstuffs serializeAddItems
        
    // Add recipes
    
    let getAddRecipesDao () = (getShoppingListActionDao (), Recipes.getDao().getByIds)
    
    let addRecipes accessToken parameters =
        addItems ShoppingLists.addRecipes accessToken parameters
        
    let addRecipesHandler ctx next =
        authorizedPostHandler (getAddRecipesDao ()) ctx next addRecipes serializeAddItems
        
    // Change foodstuff amount
    
    [<CLIMutable>]
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
        NonNegativeFloat.create amount |> mapFailure (fun _ -> [AmountMustBePositive]) |> map (fun a -> (a, foodstuff)) |> toResult |> Reader.id
        
    let private changeFoodtuffAmount accessToken foodstuff amount =
        changeAmount accessToken foodstuff amount
        |> Reader.mapEnviroment (fun dao -> dao.shoppingListAction)
        |> (Reader.map (Result.mapError (fun e -> [BusinessError(e)])))
        
    let changeAmount accessToken parameters =
        mkFoodstuff parameters.foodstuffId
        >>=! mkAmount parameters.amount
        >>=! (fun (newAmount, foodstuff) -> changeFoodtuffAmount accessToken foodstuff.id newAmount)
        
    let changeAmountHandler ctx next =
        authorizedPostHandler (getChangeAmountDao ()) ctx next changeAmount (fun a -> a)
        
    // Chnage person count
    
    [<CLIMutable>]
    type ChangePersonCountParameters = {
        recipeId: Guid
        personCount: int
    }
    
    type ChangePersonCountError = 
        | RecipeNotFound
        | PersonCountMustBePositive
        | BusinessError of UseCases.ShoppingLists.ChangeAmountError
        
    type ChangePersonCountDao = {
        shoppingListAction: ShoppingListActionDao
        recipes: RecipesDao
    }
    
    let private getChangePersonCountDao () = {
        shoppingListAction = getShoppingListActionDao ()
        recipes = Recipes.getDao ()
    }
        
    let private mkRecipe id =
        Reader(fun dao -> dao.recipes.getById id |> Option.toResult [RecipeNotFound])
        
    let private mkPersonCount personCount recipe = 
        NaturalNumber.create personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]) |> map (fun c -> (c, recipe)) |> toResult |> Reader.id
        
    let private changeRecipePersonCount accessToken recipe amount =
        changePersonCount accessToken recipe amount
        |> Reader.mapEnviroment (fun dao -> dao.shoppingListAction)
        |> (Reader.map (Result.mapError (fun e -> [BusinessError(e)])))
        
    let changePersonCount accessToken parameters =
        mkRecipe parameters.recipeId
        >>=! mkPersonCount parameters.personCount
        >>=! (fun (newPersonCount, recipe) -> changeRecipePersonCount accessToken recipe newPersonCount)
        
    let changePersonCountHandler ctx next =
        authorizedPostHandler (getChangePersonCountDao ()) ctx next changePersonCount (fun a -> a)
        
    // Cook recipe
    
    [<CLIMutable>]
    type CookRecipeParameters = {
        recipeId: Guid
    }
    
    type CookRecipeError =
        | RecipeNotFound
        | BusinessError of ShoppingLists.CookRecipeError
        
    type CookRecipeDao = {
        shoppingListAction: ShoppingListActionDao
        recipes: RecipesDao
    }
        
    let getCookRecipeDao () = {
        shoppingListAction = getShoppingListActionDao ()
        recipes = Recipes.getDao ()
    }
    
    let private getRecipe id = 
        Reader(fun dao -> dao.recipes.getById id |> Option.toResult RecipeNotFound)
        
    let private cookRecipe accessToken recipe =
        ShoppingLists.cook accessToken recipe 
        |> Reader.mapEnviroment (fun dao -> dao.shoppingListAction)
        |> Reader.map (Result.mapError (fun e -> BusinessError(e)))
        
    let cook accessToken parameters = 
        getRecipe parameters.recipeId >>=! cookRecipe accessToken
        
    let cookHandler ctx next =
        authorizedPostHandler (getCookRecipeDao ()) ctx next cook (fun a -> a)
        
    // Remove foodstuff
    
    type RemoveFoodstuffParameters = {
        foodstuffId: Guid
    }
    
    type RemoveFoodstuffError = 
        | FoodstuffNotFound
        | BusinessError of ShoppingLists.RemoveItemError
    
    type RemoveFoodstuffDao = {
        shoppingListAction: ShoppingListActionDao
        foodstuffs: FoodstuffDao
    }
    
    let private getRemoveFoodstuffDao () = {
        shoppingListAction = getShoppingListActionDao ()
        foodstuffs = Foodstuffs.getDao ()
    }
    
    let private getFoodstuffId parameters = 
        Reader(fun dao -> dao.foodstuffs.getById parameters.foodstuffId |> Option.map (fun f -> f.id) |> Option.toResult FoodstuffNotFound )
    
    let private removeFoodstuffFromList accessToken foodstuffId = 
        ShoppingLists.removeFoodstuff accessToken foodstuffId
        |> mapEnviroment (fun dao -> dao.shoppingListAction)
        |> Reader.map (Result.mapError BusinessError)
    
    let removeFoodstuff accessToken parameters = 
        getFoodstuffId parameters >>=! removeFoodstuffFromList accessToken
        
    let removeFoodstuffHandler ctx next = 
        authorizedPostHandler (getRemoveFoodstuffDao ()) ctx next removeFoodstuff (fun a -> a)
        
    // Remove recipe
    
    type RemoveRecipeParameters = {
        recipeId: Guid
    }
    
    type RemoveRecipeError = 
        | RecipeNotFound
        | BusinessError of ShoppingLists.RemoveItemError
    
    type RemoveRecipeDao = {
        shoppingListAction: ShoppingListActionDao
        recipes: RecipesDao
    }
    
    let private getRemoveRecipeDao () = {
        shoppingListAction = getShoppingListActionDao ()
        recipes = Recipes.getDao ()
    }
    
    let private getRecipeToRemove parameters = 
        Reader(fun dao -> dao.recipes.getById parameters.recipeId |> Option.toResult RecipeNotFound )
    
    let private removeRecipeFromList accessToken recipe = 
        ShoppingLists.removeRecipe accessToken recipe
        |> mapEnviroment (fun dao -> dao.shoppingListAction)
        |> Reader.map (Result.mapError BusinessError)
    
    let removeRecipe accessToken parameters = 
        getRecipeToRemove parameters >>=! removeRecipeFromList accessToken
        
    let removeRecipeHandler ctx next = 
        authorizedPostHandler (getRemoveRecipeDao ()) ctx next removeRecipe (fun a -> a)