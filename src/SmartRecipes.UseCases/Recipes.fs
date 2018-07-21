module UseCases.Recipes
    open System.Text.RegularExpressions
    open DataAccess
    open DataAccess.Context
    open FSharpPlus.Data
    open Models
    open Models.Token
    open Infrastructure
    open Models.Account
    open SmartRecipes.DataAccess
    open System
    open Infrastructure.Option
    open Users
                
    type RecipesByAccountError =
        | Unauthorized
        | UserNotFound
        
    let getAllbyAccount accessTokenValue id =
        authorize Unauthorized accessTokenValue
        |> Reader.bind (fun _ -> Users.getById (AccountId id))
        |> Reader.map (toResult UserNotFound) // TODO FIX -> always overrides unauthorized error
        |> Reader.bindResult (fun a -> Recipes.getByAccount a.id |> Reader.map Ok)
        |> Reader.execute (createDbContext ())