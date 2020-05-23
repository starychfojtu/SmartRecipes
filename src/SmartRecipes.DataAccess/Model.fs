namespace SmartRecipes.DataAccess

module Model =
    open System

    type DbAccount = {
        id: Guid
        email: string
        password: string
    }

    type DbAccessToken = {
        id: Guid
        accountId: Guid
        value: string
        expiration: DateTime
    }

    [<AllowNullLiteral>]
    type DbAmount(unit: string, value: float) =
        member val unit = unit with get, set
        member val value = value with get, set

    type DbFoodstuff = {
        id: Guid
        name: string
        baseAmount: DbAmount
        amountStep: float
    }

    type DbIngredient = {
        foodstuffId: Guid
        amount: DbAmount
        comment: string
        displayLine: string
    }

    type DbDifficulty =
        | Unspecified = 0
        | Easy = 1
        | Normal = 2
        | Hard = 3

    [<AllowNullLiteral>]
    type DbCookingTime(text: string) =
        member val text = text with get, set

    [<AllowNullLiteral>]
    type DbNutritionInfo(grams: float, percents: Nullable<int>) =
        member val grams = grams with get, set
        member val percents = percents with get, set

    type DbNutritionPerServing = {
        Calories: Nullable<int>
        Fat: DbNutritionInfo
        SaturatedFat: DbNutritionInfo
        Sugars: DbNutritionInfo
        Salt: DbNutritionInfo
        Protein: DbNutritionInfo
        Carbs: DbNutritionInfo
        Fibre: DbNutritionInfo
    }

    type DbRecipe = {
        Id: Guid
        Name: string
        CreatorId: Nullable<Guid>
        PersonCount: int
        ImageUrl: string
        Url: string
        Description: string
        Ingredients: DbIngredient seq
        Difficulty: DbDifficulty
        CookingTime: DbCookingTime
        Tags: string seq
        Rating: Nullable<int>
        NutritionPerServing: DbNutritionPerServing
    }

    type DbListItem = {
        foodstuffId: Guid
        amount: float
    }

    type DbRecipeListItem = {
        recipeId: Guid
        personCount: int
    }

    type DbShoppingList = {
        id: Guid
        accountId: Guid
        items: DbListItem seq
        recipes: DbRecipeListItem seq
    }