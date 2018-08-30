module UseCases.Recipes
    open Business
    open Business
    open Business
    open System.Text.RegularExpressions
    open DataAccess
    open DataAccess.Context
    open FSharpPlus.Data
    open Models
    open Models.Token
    open Infrastructure
    open Models.Account
    open DataAccess
    open FSharpPlus
    open Infrastructure
    open Infrastructure.Reader
    open System
    open Infrastructure.Option
    open Models.Foodstuff
    open UseCases
    open UseCases
    open Users
                
    // Get all by account
    
    type GetByAccountError =
        | Unauthorized
        | UserNotFound
        
    let private getAccount id token =
        Users.getById (AccountId id) 
        |> Reader.map (toResult UserNotFound)
        
    let private gerRecipes (account: Account) = 
        Recipes.getByAccount account.id |> Reader.map Ok
        
    let private getAllbyAccount accessTokenValue id =
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
        
    let private mapParametersTObeRefactoredTo parameters token = 
        monad {
            let foodstuffIds = Seq.map (fun i -> i.foodstuffId) parameters
            let! fooddtuffs = Foodstuffs.getByIds foodstuffIds
            // foodsutffs JOIN parameters ON FoodstuffId ... and remove Monad computation expression
        }
        
    let private createRecipe infoParameters ingredientParameters token = 
        Recipes.create token.accountId infoParameters ingredientParameters 
        |> Result.mapError (List.map InvalidParameters)
        |> Reader.id
        
    let private addRecipe recipe =
        // TODO: implement
        
    let create accessTokenValue infoParameters ingredientParameters =
        authorize [Unauthorized] accessTokenValue
        >>=! mapParameters ingredientParameters
        >>=! (fun (t, ingredientParameters) -> createRecipe infoParameters ingredientParameters t)
        >>=! addRecipe