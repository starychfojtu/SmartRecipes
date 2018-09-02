module Api.Recipes
    open Api
    open Generic
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

    let createHandler (next : HttpFunc) (ctx : HttpContext) =
        authorizedPostHandler next ctx (fun parameters accessToken ->
            Recipes.create accessToken (toInfoParemeters parameters) parameters.ingredients)