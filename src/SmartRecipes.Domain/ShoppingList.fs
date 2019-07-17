namespace SmartRecipes.Domain

module ShoppingList =
    open Account
    open Foodstuff
    open NonNegativeFloat
    open Recipe
    open NaturalNumber
    open FSharpPlus
    open Infrastructure
    open System
    open FSharpPlus.Data
    open Lens
    
    type ListItem = {
        foodstuffId: FoodstuffId
        amount: NonNegativeFloat
    }
    
    let inline _amount f item = map (fun v -> { item with amount = v }) (f item.amount)
    
    let private createItem foodstuffId amount = {
        foodstuffId = foodstuffId
        amount = amount
    }
    
    type RecipeListItem = {
        recipeId: RecipeId
        personCount: NaturalNumber
    }
    
    let inline _personCount f item = map (fun v -> { item with personCount = v }) (f item.personCount)
    
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

    let inline _items f list = map (fun v -> { list with items = v}) (f list.items)
    let inline _recipes f list = map (fun v -> { list with recipes = v}) (f list.recipes)
    let inline _mapItem key f t = map (function Some value -> Map.add key value t | None -> Map.remove key t) (f (Map.tryFind key t))
    let inline _item foodstuffId f list = (_items << (_mapItem foodstuffId)) f list
    let inline _recipe recipeId f list = (_recipes << (_mapItem recipeId)) f list
    
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
        
    let private foldBind f items list = 
        let append state item = state >>= (f item)
        Seq.fold append (Ok list) items 
    
    type AddItemError = 
        | ItemAlreadyAdded

    let addFoodstuff foodstuffId amount list =
        let tryAddItem = function
            | None -> createItem foodstuffId amount |> Some |> Ok
            | Some _ -> Error ItemAlreadyAdded
            
        (_item foodstuffId) tryAddItem list
        
    let addFoodstuffs foodstuffs list =
        foldBind (fun (f: Foodstuff) l -> addFoodstuff f.id f.baseAmount.value l) foodstuffs list
        
    let addRecipe recipe personCount list =
        let tryAddItem = function
            | None -> createRecipeItem recipe personCount |> Some |> Ok
            | Some _ -> Error ItemAlreadyAdded
            
        (_recipe recipe.Id) tryAddItem list
        
    let addRecipes recipes list =
        foldBind (fun (r: Recipe) l -> addRecipe r None l) recipes list
    
    type RemoveItemError = 
        | ItemNotInList
        
    let private tryRemoveItem = function 
        | Some _ -> None |> Ok
        | None -> Error ItemNotInList
        
    let removeFoodstuff foodstuffId list = 
        (_item foodstuffId) tryRemoveItem list
        
    let removeFoodstuffs foodstuffIds list =
        foldBind removeFoodstuff foodstuffIds list

    let removeRecipe recipeId list =
        (_recipe recipeId) tryRemoveItem list
        
    let removeRecipes recipeIds list =
        foldBind removeRecipe recipeIds list
        
    type ChangeAmountError = 
        | ItemNotInList
        
    let changeAmount foodstuffId newAmount list =
        let changeItemAmount = function 
            | Some i -> setl _amount newAmount i |> Some |> Ok
            | None -> Error ItemNotInList
            
        (_item foodstuffId) changeItemAmount list
        
    let changePersonCount recipeId newPersonCount list =
        let changeItemPersonCount = function 
            | Some i -> setl _personCount newPersonCount i |> Some |> Ok
            | None -> Error ItemNotInList
            
        (_recipe recipeId) changeItemPersonCount list