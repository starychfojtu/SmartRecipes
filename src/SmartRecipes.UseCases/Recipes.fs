module UseCases.Recipes
    open Business
    open DataAccess
    open FSharpPlus.Data
    open Infrastructure
    open Models.Account
    open FSharpPlus
    open Infrastructure.Reader
    open System
    open Infrastructure.Option
    open UseCases
    open Users
    open DataAccess.Model
    open Models.Token
    open Models.Foodstuff
    open Infrastructure.Seq
    open DataAccess.Recipes
                
    // Get all by account
    
    type GetByAccountError =
        | Unauthorized
        | UserNotFound
        
    let private getAccount usersDao id token =
        usersDao.getById (AccountId id) 
        |> Reader.map (toResult UserNotFound)
        
    let private gerRecipes recipesDao (account: Account) = 
        recipesDao.getByAccount account.id |> Reader.map Ok
        
    let getAllbyAccount tokensDao usersDao recipesDao accessTokenValue id =
        authorize tokensDao Unauthorized accessTokenValue
        >>=! getAccount usersDao id
        >>=! gerRecipes recipesDao
        
    // Create
    
//    type CreateError =
//        | Unauthorized
//        | InvalidParameters of Recipes.CreateError
//        | FoodstuffNotFound
//        
//    type IngredientParameter = {
//        foodstuffId: Guid
//        amount: float
//    }
//    
//    let private mkParameter foodstuff amount : Recipes.IngredientParameter = {
//        foodstuff = foodstuff
//        amount = amount
//    }
//    
//    let private crateParameters parameters token foodstuffs =
//        Seq.exactJoin foodstuffs (fun f -> f.id.value) parameters (fun i -> i.foodstuffId) (fun (f, p) -> mkParameter f p.amount) 
//        |> Option.map (fun f -> (token, f))
//        |> toResult [FoodstuffNotFound]
//    
//    let private mapParameters parameters token =
//        Seq.map (fun i -> i.foodstuffId) parameters
//        |> Foodstuffs.getByIds
//        |> Reader.map (crateParameters parameters token)
//
//    let private createRecipe infoParameters ingredientParameters token = 
//        Recipes.create token.accountId infoParameters ingredientParameters 
//        |> Result.mapError (List.map InvalidParameters)
//        |> Reader.id
//
//    let create accessTokenValue infoParameters ingredientParameters =
//        authorize [Unauthorized] accessTokenValue
//        >>=! mapParameters ingredientParameters
//        >>=! (fun (t, ingredientParameters) -> createRecipe infoParameters ingredientParameters t)