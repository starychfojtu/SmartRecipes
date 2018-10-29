module Api.Dto
    open System
    open System.Globalization
    open Domain
    open Domain.Account
    open Domain.Foodstuff
    open Domain.Recipe
    open Domain.ShoppingList
    open Domain.ShoppingList
    open Domain.Token
    
    let private serializeDateTime (d: DateTime) =
        d.ToString("s", CultureInfo.InvariantCulture)
    
    type AccountDto = {
        id: string
        email: string
    }
    
    let serializeAccount (account: Account) = {
        id = account.id.value.ToString ()
        email = account.credentials.email.Address
    }
    
    type AccessTokenDto = {
        value: string
        accountId: string
        expirationUtc: string
    }
    
    let serializeAccessToken (accessToken: AccessToken) = {
        value = accessToken.value.value
        accountId = accessToken.accountId.value.ToString ()
        expirationUtc = accessToken.expirationUtc |> serializeDateTime
    }
    
    type AmountDto = {
        value: float
        unit: string
    }
    
    let serializeAmount (amount: Amount) = {
        value = NonNegativeFloat.value amount.value
        unit = 
            match amount.unit with 
            | Liter -> "liter"
            | Gram -> "gram"
            | Piece -> "piece"
    }
    
    type FoodstuffDto = {
        id: string
        name: string
        amountStep: AmountDto
    }
    
    let serializeFoodstuff (foodstuff: Foodstuff) = {
        id = foodstuff.id.value.ToString ()
        name = foodstuff.name.value
        amountStep = serializeAmount foodstuff.amountStep
    }
    
    type IngredientDto = {
        foodstuffId: string
        amount: float
    }
    
    let serializeIngredient (ingredient: Ingredient) = {
        foodstuffId = ingredient.foodstuffId.value.ToString ()
        amount = NonNegativeFloat.value ingredient.amount
    }
    
    type RecipeDto = {
        id: string
        name: string
        creatorId: string
        personCount: int
        imageUrl: string
        description: string
        ingredients: seq<IngredientDto>
    }
    
    let serializeRecipe (recipe: Recipe) = {
        id = recipe.id.value.ToString ()
        name = recipe.name.value
        creatorId = recipe.creatorId.value.ToString ()
        personCount = int(recipe.personCount)
        imageUrl = recipe.imageUrl.AbsoluteUri
        description = recipe.description |> Option.map (fun d -> d.value) |> Option.defaultValue null
        ingredients = Seq.map serializeIngredient recipe.ingredients
    }
    
    type ListItemDto  = {
        foodstuffId: string
        amount: float
    }
     
    let serializeListItem (i: ListItem) = {
        foodstuffId = i.foodstuffId.value.ToString ()
        amount = NonNegativeFloat.value i.amount
    }
    
    type RecipeListItemDto  = {
        recipeId: string
        personCount: int
    }
     
    let serializeRecipeListItem (i: RecipeListItem) = {
        recipeId = i.recipeId.value.ToString ()
        personCount = int(i.personCount)
    }
    
    type ShoppingListDto = {
        id: string
        ownerId: string
        items: seq<ListItemDto>
        recipes: seq<RecipeListItemDto>
    }
    
    let serializeShoppingList (s: ShoppingList): ShoppingListDto = {
        id = s.id.value.ToString ()
        ownerId = s.accountId.value.ToString ()
        items = Seq.map serializeListItem (Map.toSeq s.items |> Seq.map (fun (k, i) -> i))
        recipes = Seq.map serializeRecipeListItem (Map.toSeq s.recipes |> Seq.map (fun (k, i) -> i))
    }