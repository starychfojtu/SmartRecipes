namespace SmartRecipes.DataAccess

module Model =
    open SmartRecipes.Domain.Foodstuff
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
        unit: MetricUnit
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
        amount: float
    }
    
    type DbRecipe = {
        id: Guid
        name: string
        creatorId: Guid
        personCount: int
        imageUrl: string
        description: string
        ingredients: seq<DbIngredient>
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
        items: seq<DbListItem>
        recipes: seq<DbRecipeListItem>
    }