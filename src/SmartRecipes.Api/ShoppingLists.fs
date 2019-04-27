namespace SmartRecipes.Api

module ShoppingLists =
    open Dto
    open SmartRecipes.DataAccess
    open SmartRecipes.DataAccess.Foodstuffs
    open SmartRecipes.DataAccess.Recipes
    open SmartRecipes.Domain
    open FSharpPlus
    open FSharpPlus.Data
    open Infrastructure
    open Infrastructure.Reader
    open Infrastructure.Validation
    open System
    open Generic
    open FSharpPlus.Data.Validation
    open SmartRecipes.UseCases.ShoppingLists
    open SmartRecipes.UseCases
    open Environment
        
    // Get 
    
    type GetShoppingListError =
        | Unauthorized
        
    let private getShoppingList accountId =
        Reader(fun env -> env.IO.ShoppingLists.get accountId |> Ok)
        
    let private serializeGet = 
        Result.map serializeShoppingList >> Result.mapError (function Unauthorized -> "Unaturhorized.")
    
    let get accessToken _ =
        Users.authorize Unauthorized accessToken
        >>=! getShoppingList
        
    let getHandler ctx next = 
        authorizedGetHandler environment ctx next get serializeGet
    
    // Add
    
    [<CLIMutable>]
    type AddItemsParameters = {
        itemIds: seq<Guid>
    }
    
    type AddItemsError =
        | InvalidIds
        | BusinessError of AddItemError
        
    let getItems parameters getByIds = Reader(fun env ->
        let itemIds = parameters.itemIds
        let items = (getByIds env) itemIds
        let foundAll = Seq.length items = Seq.length itemIds
        if foundAll
            then Ok items
            else Error InvalidIds
    )
        
    let private addItemsToShoppingList accesstToken action items = 
        action accesstToken items
        |> Reader.map (Result.mapError (fun e -> BusinessError e))
        
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

    let addItems action accessToken parameters getByIds = 
        getItems parameters getByIds >>=! addItemsToShoppingList accessToken action  
    

    // Add foodstuffs
    
    let addFoodstuffs accessToken parameters =
        addItems ShoppingLists.addFoodstuffs accessToken parameters (fun env -> env.IO.Foodstuffs.getByIds)
        
    let addFoodstuffsHandler ctx next =
        authorizedPostHandler environment ctx next addFoodstuffs serializeAddItems
        
    // Add recipes
    
    let addRecipes accessToken parameters =
        addItems ShoppingLists.addRecipes accessToken parameters (fun env -> env.IO.Recipes.getByIds)
        
    let addRecipesHandler ctx next =
        authorizedPostHandler environment ctx next addRecipes serializeAddItems
        
    // Change foodstuff amount
    
    [<CLIMutable>]
    type ChangeAmountParameters = {
        foodstuffId: Guid
        amount: float
    }
    
    type ChangeAmountError = 
        | FoodstuffNotFound
        | AmountMustBePositive
        | BusinessError of ShoppingLists.ChangeAmountError
        
    let private mkFoodstuff id =
        Reader(fun env -> env.IO.Foodstuffs.getById id |> Option.toResult [FoodstuffNotFound])
        
    let private mkAmount amount foodstuff = 
        NonNegativeFloat.create amount |> mapFailure (fun _ -> [AmountMustBePositive]) |> map (fun a -> (a, foodstuff)) |> toResult |> Reader.id
        
    let private changeFoodtuffAmount accessToken foodstuff amount =
        changeAmount accessToken foodstuff amount
        |> (Reader.map (Result.mapError (fun e -> [BusinessError(e)])))
        
    let private serializeChangeAmountError = function
        | FoodstuffNotFound -> "Foodstuff not found."
        | AmountMustBePositive -> "Amount must be positive."
        | BusinessError e ->
            match e with
            | ShoppingLists.ChangeAmountError.Unauthorized -> "Unauthorized."
            | ShoppingLists.ChangeAmountError.DomainError de ->
                match de with 
                | ItemNotInList -> "Foodstuff not in list."
                
    let private serializeChangeAmount = 
        Result.map serializeShoppingList >> Result.mapError (Seq.map serializeChangeAmountError)
        
    let changeAmount accessToken parameters =
        mkFoodstuff parameters.foodstuffId
        >>=! mkAmount parameters.amount
        >>=! (fun (newAmount, foodstuff) -> changeFoodtuffAmount accessToken foodstuff.id newAmount)
        
    let changeAmountHandler ctx next =
        authorizedPostHandler environment ctx next changeAmount serializeChangeAmount
        
    // Chnage person count
    
    [<CLIMutable>]
    type ChangePersonCountParameters = {
        recipeId: Guid
        personCount: int
    }
    
    type ChangePersonCountError = 
        | RecipeNotFound
        | PersonCountMustBePositive
        | BusinessError of ShoppingLists.ChangeAmountError
        
    let private mkRecipe id =
        Reader(fun env -> env.IO.Recipes.getById id |> Option.toResult [RecipeNotFound])
        
    let private mkPersonCount personCount recipe = 
        NaturalNumber.create personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]) |> map (fun c -> (c, recipe)) |> toResult |> Reader.id
        
    let private changeRecipePersonCount accessToken recipe amount =
        changePersonCount accessToken recipe amount
        |> (Reader.map (Result.mapError (fun e -> [BusinessError(e)])))
        
    let private serializeChangePersonCountError = function
        | RecipeNotFound -> "Recipe not found."
        | PersonCountMustBePositive -> "Person count must be positive."
        | BusinessError e ->
            match e with
            | ShoppingLists.ChangeAmountError.Unauthorized -> "Unauthorized."
            | ShoppingLists.ChangeAmountError.DomainError de ->
                match de with 
                | ItemNotInList -> "Recipe not in list."
                
    let private serializeChangePersonCount = 
        Result.map serializeShoppingList >> Result.mapError (Seq.map serializeChangePersonCountError)
        
    let changePersonCount accessToken parameters =
        mkRecipe parameters.recipeId
        >>=! mkPersonCount parameters.personCount
        >>=! (fun (newPersonCount, recipe) -> changeRecipePersonCount accessToken recipe newPersonCount)
        
    let changePersonCountHandler ctx next =
        authorizedPostHandler environment ctx next changePersonCount serializeChangePersonCount
        
    // Cook recipe
    
    [<CLIMutable>]
    type CookRecipeParameters = {
        recipeId: Guid
    }
    
    type CookRecipeError =
        | RecipeNotFound
        | BusinessError of ShoppingLists.CookRecipeError
        
    let private getRecipe id = 
        Reader(fun env -> env.IO.Recipes.getById id |> Option.toResult RecipeNotFound)
        
    let private cookRecipe accessToken recipe =
        ShoppingLists.cook accessToken recipe
        |> Reader.map (Result.mapError (fun e -> BusinessError(e)))
        
    let private serializeCookRecipeError = function 
        | RecipeNotFound -> "Recipe not found."
        | BusinessError e -> 
            match e with 
            | ShoppingLists.CookRecipeError.Unauthorized -> "Unauthorized."
            | ShoppingLists.CookRecipeError.DomainError de ->
                match de with 
                | RecipeNotInList -> "Recipe not in list."
                | NotEnoughIngredientsInList -> "Not enough ingredients."
        
    let private serializeCookRecipe =
        Result.map serializeShoppingList >> Result.mapError serializeCookRecipeError
        
    let cook accessToken parameters = 
        getRecipe parameters.recipeId >>=! cookRecipe accessToken
        
    let cookHandler ctx next =
        authorizedPostHandler environment ctx next cook serializeCookRecipe
        
    // Remove foodstuff
    
    type RemoveFoodstuffParameters = {
        foodstuffId: Guid
    }
    
    type RemoveFoodstuffError = 
        | FoodstuffNotFound
        | BusinessError of ShoppingLists.RemoveItemError
    
    let private getFoodstuffId parameters = 
        Reader(fun env -> env.IO.Foodstuffs.getById parameters.foodstuffId |> Option.map (fun f -> f.id) |> Option.toResult FoodstuffNotFound )
    
    let private removeFoodstuffFromList accessToken foodstuffId = 
        ShoppingLists.removeFoodstuff accessToken foodstuffId
        |> Reader.map (Result.mapError BusinessError)
        
    let private serializeRemoveFoodstuffError = function 
        | FoodstuffNotFound -> "Foodstuff not found."
        | BusinessError e -> 
            match e with
            | ShoppingLists.RemoveItemError.Unauthorized -> "Unauthorized."
            | ShoppingLists.RemoveItemError.DomainError de ->
                match de with 
                | ItemNotInList -> "Foodstuff not in list."
        
    let private serializeRemoveFoodstuff = 
        Result.map serializeShoppingList >> Result.mapError serializeRemoveFoodstuffError
    
    let removeFoodstuff accessToken parameters = 
        getFoodstuffId parameters >>=! removeFoodstuffFromList accessToken
        
    let removeFoodstuffHandler ctx next = 
        authorizedPostHandler environment ctx next removeFoodstuff serializeRemoveFoodstuff
        
    // Remove recipe
    
    type RemoveRecipeParameters = {
        recipeId: Guid
    }
    
    type RemoveRecipeError = 
        | RecipeNotFound
        | BusinessError of ShoppingLists.RemoveItemError
    
    let private getRecipeToRemove parameters = 
        Reader(fun env -> env.IO.Recipes.getById parameters.recipeId |> Option.toResult RecipeNotFound )
    
    let private removeRecipeFromList accessToken recipe = 
        ShoppingLists.removeRecipe accessToken recipe
        |> Reader.map (Result.mapError BusinessError)
        
    let private serializeRemoveRecipeError = function 
        | RecipeNotFound -> "Recipe not found."
        | BusinessError e -> 
            match e with
            | ShoppingLists.RemoveItemError.Unauthorized -> "Unauthorized."
            | ShoppingLists.RemoveItemError.DomainError de ->
                match de with 
                | ItemNotInList -> "Recipe not in list."
        
    let private serializeRemoveRecipe = 
        Result.map serializeShoppingList >> Result.mapError serializeRemoveRecipeError
    
    let removeRecipe accessToken parameters = 
        getRecipeToRemove parameters >>=! removeRecipeFromList accessToken
        
    let removeRecipeHandler ctx next = 
        authorizedPostHandler environment ctx next removeRecipe serializeRemoveRecipe