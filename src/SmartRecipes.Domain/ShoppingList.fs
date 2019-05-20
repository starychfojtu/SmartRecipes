namespace SmartRecipes.Domain

module ShoppingList =
    open Account
    open Foodstuff
    open NonNegativeFloat
    open Recipe
    open NaturalNumber
    open FSharpPlus
    open Infrastructure
    open Infrastracture.Result
    open System
    open FSharpPlus.Data
    
    type ListItem = {
        foodstuffId: FoodstuffId
        amount: NonNegativeFloat
    }
    
    let private createItem foodstuffId amount = {
        foodstuffId = foodstuffId
        amount = amount
    }
    
    type RecipeListItem = {
        recipeId: RecipeId
        personCount: NaturalNumber
    }
    
    let private createRecipeItem recipe personCount = {
        recipeId = recipe.Id
        personCount = Option.defaultValue recipe.PersonCount personCount
    }
        
    type ShoppingListId = ShoppingListId of Guid
        with member i.value = match i with ShoppingListId v -> v
    
    type ShoppingList = {
        id: ShoppingListId
        accountId: AccountId
        items: Map<FoodstuffId, ListItem>
        recipes: Map<RecipeId, RecipeListItem>
    }

    let create accountId = {
        id = ShoppingListId(Guid.NewGuid ())
        accountId = accountId
        items = Map.empty
        recipes = Map.empty
    }
    
    let findItem foodstuffId list =
        Map.tryFind foodstuffId list.items
        
    let findRecipeItem (recipe: Recipe) list =
        Map.tryFind recipe.Id list.recipes
    
    type AddItemError = 
        | ItemAlreadyAdded

    let addFoodstuff list foodstuffId amount =
        let existingItem = findItem foodstuffId list
        match existingItem with 
        | Some i -> Error ItemAlreadyAdded
        | None -> Ok { list with items = Map.add foodstuffId (createItem foodstuffId amount) list.items }
        
    let addFoodstuffs list foodstuffs = 
        let initState = Ok list
        let append state (foodstuff: Foodstuff) = Result.bind (fun s -> addFoodstuff s foodstuff.id foodstuff.baseAmount.value) state
        Seq.fold append initState foodstuffs 
        
    let addRecipe list recipe personCount =
        let existingItem = findRecipeItem recipe list
        match existingItem with 
        | Some i -> Error ItemAlreadyAdded
        | None -> Ok { list with recipes = Map.add recipe.Id (createRecipeItem recipe personCount) list.recipes }
        
    let addRecipes list recipes = 
        let initState = Ok list
        let append state recipe = Result.bind (fun s -> addRecipe s recipe None) state
        Seq.fold append initState recipes 
    
    type RemoveItemError = 
        | ItemNotInList
        
    let removeFoodstuff list foodstuffId = 
        let existingItem = findItem foodstuffId list
        match existingItem with 
        | Some i -> Ok { list with items = Map.remove foodstuffId list.items }
        | None -> Error ItemNotInList

    let removeRecipe list recipe = 
        let existingItem = findRecipeItem recipe list
        match existingItem with
        | Some i -> Ok { list with recipes = Map.remove recipe.Id list.recipes }
        | None -> Error ItemNotInList
        
    type ChangeAmountError = 
        | ItemNotInList
        
    let changeAmount foodstuffId newAmount list =
        removeFoodstuff list foodstuffId
        |> Result.mapError (fun _ -> ItemNotInList)
        |> Result.map (fun l -> addFoodstuff l foodstuffId newAmount |> forceOk)
        
    let changePersonCount recipe newPersonCount list =
        removeRecipe list recipe
        |> Result.mapError (fun _ -> ItemNotInList)
        |> Result.map (fun l -> addRecipe l recipe (Some newPersonCount) |> forceOk)
        
    type DecreaseAmountError = 
        | ItemNotInList
        | NotEnoughAmountInList
        
    let private findCurrentItem foodstuffId list =
        findItem foodstuffId list |> Option.toResult ItemNotInList
        
    let private getDifference delta item =
        failwith "NotImplemented" // TODO: Needs foodstuff injected and proper conversions per foodstuffs to work at all.
        
    let private decrease foodstuffId list newAmount =
        changeAmount foodstuffId newAmount list |> Result.mapError (fun _ -> ItemNotInList)
        
    let decreaseAmount foodstuffId amount list =
        findCurrentItem foodstuffId list
        >>= getDifference amount
        >>= decrease foodstuffId list
        
    type CookRecipeError = 
        | RecipeNotInList
        | NotEnoughIngredientsInList
        
    let private isRecipeInList recipe list =
        match findRecipeItem recipe list with | Some _ -> true | None -> false
        
    let private decreaseIngredient (ingredient: Ingredient) list = 
        decreaseAmount ingredient.FoodstuffId ingredient.Amount list |> Result.mapError (fun _ -> NotEnoughIngredientsInList)
        
    let private decreaseIngredientAmounts recipe list =
        let ingredients = NonEmptyList.toSeq recipe.Ingredients
        let initState = (Ok list)
        Seq.fold (fun list (i: Ingredient) -> Result.bind (fun l -> decreaseIngredient i l) list) initState ingredients 
        
    let cook recipe list =
        if isRecipeInList recipe list
            then decreaseIngredientAmounts recipe list |> Result.map (fun l -> removeRecipe l recipe |> forceOk)
            else Error RecipeNotInList
        