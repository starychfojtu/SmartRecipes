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
    open System
    open Infrastructure.Option
    open Models.Foodstuff
    open UseCases
    open Users
                
    type GetByAccountError =
        | Unauthorized
        | UserNotFound
        
    let getAllbyAccount accessTokenValue id =
        authorize Unauthorized accessTokenValue
        |> Reader.bindResult (fun _ -> Users.getById (AccountId id) |> Reader.map Ok)
        |> Reader.map (Result.bind (toResult UserNotFound))
        |> Reader.bindResult (fun a -> Recipes.getByAccount a.id |> Reader.map Ok)
        |> Reader.execute (createDbContext ())
        
    type CreateError =
        | Unauthorized
        | InvalidParameters of Recipes.CreateError
        | FoodstuffNotFound
        
    type IngredientParameter = {
        foodstuffId: Guid
        amount: float
    }
    
    let mkParameter foodstuff amount : Recipes.IngredientParameter = {
        foodstuff = foodstuff
        amount = amount
    }
        
    let getIngredientParametersWithFoodstuff parameters = 
        monad {
            let foodstuffIds = Seq.map (fun i -> i.foodstuffId) parameters
            let! fooddtuffs = Foodstuffs.getByIds foodstuffIds
            let tupledFoodstuff = Seq.map (fun f -> (f.id.value, f)) fooddtuffs
            let foodstuffMap = Map.ofSeq tupledFoodstuff
            return 
                if Seq.length parameters = Seq.length fooddtuffs
                    then Seq.map (fun p -> mkParameter (Map.find p.foodstuffId foodstuffMap) p.amount) parameters |> Ok
                    else Error [FoodstuffNotFound]
        }
        
    // TODO: refactor this whole thing
    let create accessTokenValue infoParameters ingredientParameters =
        authorize [Unauthorized] accessTokenValue
        |> Reader.bindResult (fun t -> 
            getIngredientParametersWithFoodstuff ingredientParameters
            |> Reader.map (Result.bind (fun is -> 
                (Recipes.create t.accountId infoParameters is 
                    |> Result.mapError (List.map InvalidParameters)))))
            // |> Reader.bindResult (fun r -> Recipes.add r |> Reader.map Ok))
        |> Reader.execute (createDbContext ())