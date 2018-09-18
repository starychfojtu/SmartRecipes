module Api.Dto
    open Domain.Account
    open Domain.Foodstuff
    open Domain.Recipe
    open Domain.Token
    
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
    }
    
    let serializeAccessToken (accessToken: AccessToken) = {
        value = accessToken.value.value
        accountId = accessToken.accountId.value.ToString ()
    }
    
    type AmountDto = {
        value: float
        unit: string
    }
    
    let serializeAmount (amount: Amount) = {
        value = amount.value.value
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
        amount = ingredient.amount.value
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