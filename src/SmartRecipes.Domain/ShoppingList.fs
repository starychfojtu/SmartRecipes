module Domain.ShoppingList
    open Domain
    open Domain.Account
    open Domain.Foodstuff
    open Domain.NonNegativeFloat
    open Domain.Recipe
    open Domain.NaturalNumber
    open FSharpPlus
    open Infrastructure
    open Infrastracture.Result
    open System
    open FSharpPlus.Data
    
    type ListItem = {
        foodstuffId: FoodstuffId
        amount: NonNegativeFloat
    }
    
    let private createItem (foodstuff: Foodstuff) amount = {
        foodstuffId = foodstuff.id
        amount = Option.defaultValue foodstuff.baseAmount.value amount
    }
    
    type RecipeListItem = {
        recipeId: RecipeId
        personCount: NaturalNumber
    }
    
    let private createRecipeItem recipe personCount = {
        recipeId = recipe.id
        personCount = Option.defaultValue recipe.personCount personCount
    }
        
    type ShoppingListId = ShoppingListId of Guid
    
    type ShoppingList = {
        id: ShoppingListId
        accountId: AccountId
        items: Map<FoodstuffId, ListItem>
        recipes: Map<RecipeId, RecipeListItem>
    }

    let createShoppingList accountId = {
        id = ShoppingListId(Guid.NewGuid ())
        accountId = accountId
        items = Map.empty
        recipes = Map.empty
    }
    
    let findItem (foodstuff: Foodstuff) list =
        Map.tryFind foodstuff.id list.items
        
    let findRecipeItem (recipe: Recipe) list =
        Map.tryFind recipe.id list.recipes
    
    type AddItemError = 
        | ItemAlreadyAdded

    let addFoodstuff list foodstuff amount =
        let existingItem = findItem foodstuff list
        match existingItem with 
        | Some i -> Error ItemAlreadyAdded
        | None -> Ok { list with items = Map.add foodstuff.id (createItem foodstuff amount) list.items }
        
    let addFoodstuffs list foodstuffs = 
        let initState = Ok list
        let append state foodstuff = Result.bind (fun s -> addFoodstuff s foodstuff None) state
        Seq.fold append initState foodstuffs 
        
    let addRecipe list recipe personCount =
        let existingItem = findRecipeItem recipe list
        match existingItem with 
        | Some i -> Error ItemAlreadyAdded
        | None -> Ok { list with recipes = Map.add recipe.id (createRecipeItem recipe personCount) list.recipes }
        
    let addRecipes list recipes = 
        let initState = Ok list
        let append state recipe = Result.bind (fun s -> addRecipe s recipe None) state
        Seq.fold append initState recipes 
    
    type RemoveItemError = 
        | ItemNotFound
        
    let removeFoodstuff list foodstuff = 
        let existingItem = findItem foodstuff list
        match existingItem with 
        | Some i -> Ok { list with items = Map.remove foodstuff.id list.items }
        | None -> Error ItemNotFound

    let removeRecipe list recipe = 
        let existingItem = findRecipeItem recipe list
        match existingItem with
        | Some i -> Ok { list with recipes = Map.remove recipe.id list.recipes }
        | None -> Error ItemNotFound
        
    type ChangeAmountError = 
        | ItemNotFound
        
    let changeAmount foodstuff newAmount list =
        removeFoodstuff list foodstuff
        |> Result.mapError (fun _ -> ItemNotFound)
        |> Result.map (fun l -> addFoodstuff l foodstuff (Some newAmount) |> forceOk)
        
    let changePersonCount recipe newPersonCount list =
        removeRecipe list recipe
        |> Result.mapError (fun _ -> ItemNotFound)
        |> Result.map (fun l -> addRecipe l recipe (Some newPersonCount) |> forceOk)