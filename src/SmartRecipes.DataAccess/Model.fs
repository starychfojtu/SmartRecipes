namespace SmartRecipes.DataAccess
open SmartRecipes.Domain.Recipe

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
        
    type DbAmount = {
        unit: string
        value: float
    }

    type DbFoodstuff = {
        id: Guid
        name: string
        baseAmount: DbAmount
        amountStep: float
    }
    
    type DbIngredient = {
        foodstuffId: Guid
        amount: DbAmount option
        comment: string
        displayLine: string
    }
    
    type DbDifficulty =
        | Easy = 0
        | Normal = 1
        | Hard = 2
        
    type DbCookingTime = {
        Text: string
    }
    
    type DbNutritionInfo = {
        Grams: int
        Percents: int option
    }
    
    type DbNutritionPerServing = {
        Calories: int option
        Fat: DbNutritionInfo option
        SaturatedFat: DbNutritionInfo option
        Sugars: DbNutritionInfo option
        Protein: DbNutritionInfo option
        Carbs: DbNutritionInfo option
    }
    
    type DbRecipe = {
        Id: Guid
        Name: string
        CreatorId: Guid
        PersonCount: int
        ImageUrl: string
        Url: string
        Description: string
        Ingredients: DbIngredient seq
        Difficulty: DbDifficulty option
        CookingTime: DbCookingTime option
        Tags: string seq
        Rating: int option
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