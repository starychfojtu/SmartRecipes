module DataAccess.Model
    open System.Collections.Generic
    open System
    
    type DbAccount = {
        id: Guid
        email: string
        password: string
    }

    type DbAccessToken = {
        accountId: Guid
        value: string
        expiration: DateTime
    }
    
    type DbMetricUnit = 
        | Liter = 0
        | Gram = 1
        | Piece = 2
        
    type DbAmount = {
        unit: DbMetricUnit
        value: float
    }

    type DbFoodstuff = {
        id: Guid
        name: string
        baseAmount: DbAmount
        amountStep: DbAmount
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