module Api.Recipes
    open Models.Account
    open System
    open Giraffe
    open Microsoft.AspNetCore.Http
    open UseCases
    
    type IndexParameters = {
        accountId: Guid
    }
    
    let indexHandler (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! parameters = ctx.BindModelAsync<IndexParameters>()
            let accessToken = ""
            let recipes = Recipes.getAllbyAccount accessToken parameters.accountId
            return! json recipes next ctx
        }