module UseCases.Recipes
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
        | InvalidRecipeParameters
        | FoodstuffNotFound
        
    let getFoodstuffs foodstuffIds = 
        Foodstuffs.getByIds foodstuffIds
        |> Reader.map (fun fs -> 
            if Seq.length fs = Seq.length foodstuffIds 
                then Ok fs 
                else Error FoodstuffNotFound)
        
    let create accessTokenValue parameters ingredients =
        let foodstuffIds = Seq.map (fun i -> i.foodstuffId) ingredients
        authorize Unauthorized accessTokenValue
        |> Reader.bindResult (fun _ -> ensurefoodstuffsExist foodstuffIds)
        |> Reader.bindResult (fun _ -> Recipes.create parameters ingredients)
        |> Reader.bindResult (fun r -> Recipes.add r |> Reader.map Ok)
        |> Reader.execute (createDbContext ())