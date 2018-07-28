module DataAccess.Model
    open System

    [<CLIMutable>]
    type DbAccount = {
        id: Guid;
        email: string;
        password: string;
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
        unit: DbMetricUnit;
        value: float
    }
    
    [<CLIMutable>]
    type DbFoodstuff = {
        id: Guid
        name: string;
        baseAmount: DbAmount
        amountStep: DbAmount
    }
    
    [<CLIMutable>]
    type DbRecipe = {
        id: Guid;
        name: string;
        creatorId: Guid;
    }
    
    [<CLIMutable>]
    type DbIngredient = {
        id: Guid;
        recipeId: Guid;
        foodstuffId: Guid;
        amount: DbAmount;
    }