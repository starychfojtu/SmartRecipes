module Api.Recipes
    open Models.Account
    open System
    open Giraffe
    open Microsoft.AspNetCore.Http
    open UseCases
    
    [<CLIMutable>]
    type IndexParameters = {
        accountId: Guid
    }
    
    let indexHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let parameters = ctx.BindQueryString<IndexParameters>()
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            let recipes = Recipes.getAllbyAccount accessToken parameters.accountId
            return! json recipes next ctx
        }