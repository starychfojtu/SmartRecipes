module DataAccess.Model
    open System.Collections.Generic
    open System
    open MongoRepository
    
    type DbAccount(id: Guid, email: string, password: string) =
        inherit Entity()
        member this.Id = id
        member this.Email = email
        member this.Password = password

    type DbAccessToken(accountId: Guid, value: string, expiration: DateTime) =
        inherit Entity()
    
    type DbMetricUnit = 
        | Liter = 0
        | Gram = 1
        | Piece = 2
        
    type DbAmount(unit: DbMetricUnit, value: float) =
        inherit obj()

    type DbFoodstuff(id: Guid, name: string, baseAmount: DbAmount, amountStep: DbAmount) =
        inherit Entity()
    
    type DbIngredient(foodstuffId: Guid, amount: float) =
        inherit obj()

    type DbRecipe(id: Guid, name: string, creatorId: Guid, personCount: int, imageUrl: string, description: string, ingredients: seq<DbIngredient>) = 
        inherit Entity()
        
    type DbListItem(foodstuffId: Guid, amount: float) = 
        inherit obj()
        
    type DbRecipeListItem(recipeId: Guid, personCount: int) = 
        inherit obj()
        
    type DbShoppingList(id: Guid, accountId: Guid, items: seq<DbListItem>, recipes: seq<DbRecipeListItem>) =
        inherit Entity()