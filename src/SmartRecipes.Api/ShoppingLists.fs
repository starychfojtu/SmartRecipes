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
    
    let shoppingListActionDao: ShoppingListActionDao = {
        tokens = Tokens.dao
        shoppingLists = ShoppingLists.dao
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
        authorizedGetHandler shoppingListActionDao ctx next get serializeGet
    
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
    
    let private addFoodstuffDao = (shoppingListActionDao, Foodstuffs.dao.getByIds)
    
    let addFoodstuffs accessToken parameters =
        addItems ShoppingLists.addFoodstuffs accessToken parameters
        
    let addFoodstuffsHandler ctx next =
        authorizedPostHandler addFoodstuffDao ctx next addFoodstuffs serializeAddItems
        
    // Add recipes
    
    let addRecipesDao = (shoppingListActionDao, Recipes.dao.getByIds)
    
    let addRecipes accessToken parameters =
        addItems ShoppingLists.addRecipes accessToken parameters
        
    let addRecipesHandler ctx next =
        authorizedPostHandler addRecipesDao ctx next addRecipes serializeAddItems
        
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
        
    type ChangeAmountDao = {
        shoppingListAction: ShoppingListActionDao
        foodstuffs: FoodstuffDao
    }
    
    let private changeAmountDao = {
        shoppingListAction = shoppingListActionDao
        foodstuffs = Foodstuffs.dao
    }
        
    let private mkFoodstuff id =
        Reader(fun dao -> dao.foodstuffs.getById id |> Option.toResult [FoodstuffNotFound])
        
    let private mkAmount amount foodstuff = 
        NonNegativeFloat.create amount |> mapFailure (fun _ -> [AmountMustBePositive]) |> map (fun a -> (a, foodstuff)) |> toResult |> Reader.id
        
    let private changeFoodtuffAmount accessToken foodstuff amount =
        changeAmount accessToken foodstuff amount
        |> Reader.mapEnviroment (fun dao -> dao.shoppingListAction)
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
        authorizedPostHandler changeAmountDao ctx next changeAmount serializeChangeAmount
        
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
        
    type ChangePersonCountDao = {
        shoppingListAction: ShoppingListActionDao
        recipes: RecipesDao
    }
    
    let private changePersonCountDao = {
        shoppingListAction = shoppingListActionDao
        recipes = Recipes.dao
    }
        
    let private mkRecipe id =
        Reader(fun dao -> dao.recipes.getById id |> Option.toResult [RecipeNotFound])
        
    let private mkPersonCount personCount recipe = 
        NaturalNumber.create personCount |> mapFailure (fun _ -> [PersonCountMustBePositive]) |> map (fun c -> (c, recipe)) |> toResult |> Reader.id
        
    let private changeRecipePersonCount accessToken recipe amount =
        changePersonCount accessToken recipe amount
        |> Reader.mapEnviroment (fun dao -> dao.shoppingListAction)
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
        authorizedPostHandler changePersonCountDao ctx next changePersonCount serializeChangePersonCount
        
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
        
    let cookRecipeDao = {
        shoppingListAction = shoppingListActionDao
        recipes = Recipes.dao
    }
    
    let private getRecipe id = 
        Reader(fun dao -> dao.recipes.getById id |> Option.toResult RecipeNotFound)
        
    let private cookRecipe accessToken recipe =
        ShoppingLists.cook accessToken recipe 
        |> Reader.mapEnviroment (fun dao -> dao.shoppingListAction)
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
        authorizedPostHandler cookRecipeDao ctx next cook serializeCookRecipe
        
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
    
    let private removeFoodstuffDao = {
        shoppingListAction = shoppingListActionDao
        foodstuffs = Foodstuffs.dao
    }
    
    let private getFoodstuffId parameters = 
        Reader(fun dao -> dao.foodstuffs.getById parameters.foodstuffId |> Option.map (fun f -> f.id) |> Option.toResult FoodstuffNotFound )
    
    let private removeFoodstuffFromList accessToken foodstuffId = 
        ShoppingLists.removeFoodstuff accessToken foodstuffId
        |> mapEnviroment (fun dao -> dao.shoppingListAction)
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
        authorizedPostHandler removeFoodstuffDao ctx next removeFoodstuff serializeRemoveFoodstuff
        
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
    
    let private removeRecipeDao = {
        shoppingListAction = shoppingListActionDao
        recipes = Recipes.dao
    }
    
    let private getRecipeToRemove parameters = 
        Reader(fun dao -> dao.recipes.getById parameters.recipeId |> Option.toResult RecipeNotFound )
    
    let private removeRecipeFromList accessToken recipe = 
        ShoppingLists.removeRecipe accessToken recipe
        |> mapEnviroment (fun dao -> dao.shoppingListAction)
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
        authorizedPostHandler removeRecipeDao ctx next removeRecipe serializeRemoveRecipe