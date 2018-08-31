module Api.Recipes
    open DataAccess
    open System
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open UseCases
    open Context
    
    [<CLIMutable>]
    type IndexParameters = {
        accountId: Guid
    }
    
    let indexHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let parameters = ctx.BindQueryString<IndexParameters>()
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            let recipes = Recipes.getAllbyAccount accessToken parameters.accountId |> Reader.execute (createDbContext())
            return! json recipes next ctx
        }