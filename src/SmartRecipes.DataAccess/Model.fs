module DataAccess.Model
    open System.Collections.Generic
    open System

    [<CLIMutable>]
    type DbAccount = {
        id: Guid
        email: string
        password: string
    }
    
    [<CLIMutable>]
    type DbAccessToken = {
        accountId: Guid
        value: string
        expiration: DateTime
    }
    
    type DbMetricUnit = 
        | Liter = 0
        | Gram = 1
        | Piece = 2
    
    [<CLIMutable>]
    type DbAmount = {
        unit: DbMetricUnit
        value: float
    }
    
    [<CLIMutable>]
    type DbFoodstuff = {
        id: Guid
        name: string
        baseAmount: DbAmount
        amountStep: DbAmount
    }
    
    [<CLIMutable>]
    type DbIngredient = {
        id: string
        recipeId: Guid
        foodstuffId: Guid
        foodstuff: DbFoodstuff
        amount: float
    }
    
    [<CLIMutable>]
    type DbRecipe = {
        id: Guid
        name: string
        creatorId: Guid
        personCount: int
        imageUrl: string
        description: string
        ingredients: IEnumerable<DbIngredient>
    }
    
    [<CLIMutable>]
    type DbListItem = {
        listId: Guid
        foodstuffId: Guid
        amount: float
    }
    
    [<CLIMutable>]
    type DbRecipeListItem = {
        listId: Guid
        recipeId: Guid
        personCount: int
    }
    
    [<CLIMutable>]
    type DbShoppingList = {
        id: Guid
        accountId: Guid
        items: seq<DbListItem>
        recipes: seq<DbRecipeListItem>
    }