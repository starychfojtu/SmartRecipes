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
                
    // Get all by account
    
    type GetByAccountError =
        | Unauthorized
        | UserNotFound
        
    let private getAccount id token =
        Users.getById (AccountId id) 
        |> Reader.map (toResult UserNotFound)
        
    let private gerRecipes (account: Account) = 
        Recipes.getByAccount account.id |> Reader.map Ok
        
    let getAllbyAccount accessTokenValue id =
        authorize Unauthorized accessTokenValue
        >>=! getAccount id
        >>=! gerRecipes
        
    // Create
    
    type CreateError =
        | Unauthorized
        | InvalidParameters of Recipes.CreateError
        | FoodstuffNotFound
        
    type IngredientParameter = {
        foodstuffId: Guid
        amount: float
    }
    
    let private mkParameter foodstuff amount : Recipes.IngredientParameter = {
        foodstuff = foodstuff
        amount = amount
    }
      
    // TODO: Implement Join, refactor this
    let private mapParameters parameters token = 
        monad {
            let foodstuffIds = Seq.map (fun i -> i.foodstuffId) parameters
            let! fooddtuffs = Foodstuffs.getByIds foodstuffIds
            let tupledFoodstuff = Seq.map (fun f -> (f.id.value, f)) fooddtuffs
            let foodstuffMap = Map.ofSeq tupledFoodstuff
            return 
                if Seq.length parameters = Seq.length fooddtuffs
                    then (token, Seq.map (fun p -> mkParameter (Map.find p.foodstuffId foodstuffMap) p.amount) parameters) |> Ok
                    else Error [FoodstuffNotFound]
        }

    let private createRecipe infoParameters ingredientParameters token = 
        Recipes.create token.accountId infoParameters ingredientParameters 
        |> Result.mapError (List.map InvalidParameters)
        |> Reader.id

    let create accessTokenValue infoParameters ingredientParameters =
        authorize [Unauthorized] accessTokenValue
        >>=! mapParameters ingredientParameters
        >>=! (fun (t, ingredientParameters) -> createRecipe infoParameters ingredientParameters t)