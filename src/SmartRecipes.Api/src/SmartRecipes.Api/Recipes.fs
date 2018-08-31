module Api.Recipes
    open System.Net.Http
    open Business
    open DataAccess
    open System
    open Giraffe
    open Infrastructure
    open Microsoft.AspNetCore.Http
    open UseCases
    open Context
    open FSharpPlus.Data
    open UseCases.Recipes
    
    // TODO: move to utils
    let authorizedGetHandler<'parameters, 'result, 'error> (next : HttpFunc) (ctx : HttpContext) (handler: 'parameters -> string -> Reader<Context, Result<'result, 'error>>) = 
        task {
            let parameters = ctx.BindQueryString<'parameters>()
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            let result = handler parameters accessToken |> Reader.execute (createDbContext())
            return! json result next ctx
        }
        
    let authorizedPostHandler<'parameters, 'result, 'error> (next : HttpFunc) (ctx : HttpContext) (handler: 'parameters -> string -> Reader<Context, Result<'result, 'error>>) = 
        task {
            let! parameters = ctx.BindModelAsync<'parameters>()
            let accessToken = match ctx.GetRequestHeader("authorization") with Ok t -> t | Error _ -> ""
            let result = handler parameters accessToken |> Reader.execute (createDbContext())
            return! json result next ctx
        }
            
    [<CLIMutable>]
    type IndexParameters = {
        accountId: Guid
    }
    
    let indexHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedGetHandler next ctx (fun parameters accessToken ->
            Recipes.getAllbyAccount accessToken parameters.accountId)
        
    [<CLIMutable>]
    type CreateParameters = {
        name: string
        personCount: int
        description: string
        imageUrl: string
        ingredients: seq<IngredientParameter>
    }
    
    let toInfoParemeters createParameters: Recipes.RecipeInfoParameters = {
        name = createParameters.name
        personCount = createParameters.personCount
        description = createParameters.description
        imageUrl = createParameters.imageUrl
    }
        
    // TODO: Add validation for parameters that they are provided at all -> ingredients cacnnot be null
    let createHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedPostHandler next ctx (fun parameters accessToken ->
            Recipes.create accessToken (toInfoParemeters parameters) parameters.ingredients)