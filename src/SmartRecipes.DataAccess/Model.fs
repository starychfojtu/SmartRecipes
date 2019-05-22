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