module Domain.ShoppingList
    open Domain
    open Domain.Account
    open Domain.Foodstuff
    open Domain.NonNegativeFloat
    open Domain.Recipe
    open Domain.NaturalNumber
    open Infrastructure
    open System
    
    type ListItem = {
        foodstuffId: FoodstuffId
        amount: NonNegativeFloat
    }
    
    let private createItem (foodstuff: Foodstuff) = {
        foodstuffId = foodstuff.id
        amount = foodstuff.baseAmount.value
    }
    
    type RecipeListItem = {
        recipeId: RecipeId
        personCount: NaturalNumber
    }
    
    let private createRecipeItem recipe = {
        recipeId = recipe.id
        personCount = recipe.personCount
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
    
    type AddItemError = 
        | ItemAlreadyAdded

    let addItem list (foodstuff: Foodstuff) =
        let existingItem = Map.tryFind foodstuff.id list.items
        match existingItem with 
        | Some i -> Error ItemAlreadyAdded
        | None -> Ok { list with items = Map.add foodstuff.id (createItem foodstuff) list.items }
        
    let addRecipe list (recipe: Recipe) =
        let existingItem = Map.tryFind recipe.id list.recipes
        match existingItem with 
        | Some i -> Error ItemAlreadyAdded
        | None -> Ok { list with recipes = Map.add recipe.id (createRecipeItem recipe) list.recipes }
    
    type RemoveItemError = 
        | ItemNotFound
        
    let removeItem list (foodstuff: Foodstuff) = 
        let existingItem = Map.tryFind foodstuff.id list.items
        match existingItem with 
        | Some i -> Ok { list with items = Map.remove foodstuff.id list.items }
        | None -> Error ItemNotFound

    let removeRecipeItem list (recipe: Recipe) = 
        let existingItem = Map.tryFind recipe.id list.recipes
        match existingItem with
        | Some i -> Ok { list with recipes = Map.remove recipe.id list.recipes }
        | None -> Error ItemNotFound