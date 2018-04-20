namespace SmartRecipes.Api

open Giraffe
open System
open SmartRecipes.DataAccess
open Microsoft.AspNetCore.Http
open SmartRecipes.Business
open Generic

[<RequireQualifiedAccess>]
module Recipes =

    let index next ctx =
        json [] next ctx
        // authorize4
        //>> Recipes.query.byAccount account
        // >> json

    let detail id =
        Generic.detail Recipes.query.withId id

    // Create TODO: make much more generic

    type createParameters = {
        name: string;
    }

    type createErrorMessage = 
        | ParametersNotProvided
        | NameCannotBeEmpty

    let nameNotNull parameters = 
        match String.IsNullOrWhiteSpace(parameters.name) with
        | true -> Failure NameCannotBeEmpty
        | false -> Success parameters
        
    let validateCreateParameters =
        nameNotNull

    let createRecipe parameters =
        Recipes.create parameters.name |> Success

    let addRecipe recipe =
        Recipes.add recipe |> Success

    let create (next: HttpFunc) (ctx: HttpContext) =
        let pipe =
            bindParameters
            >>= validateCreateParameters
            >>= createRecipe
            >>= addRecipe
            >> showResponse
        pipe ctx next ctx

(*        let update (ctx: HttpContext) (next: HttpFunc) = 
            Recipes.update recipe command

        let delete (ctx: HttpContext) (next: HttpFunc) =
            Recipes.delete recipe command
*)