namespace SmartRecipes.Api.HttpHandlers

open Giraffe
open SmartRecipes.Models
open System
open SmartRecipes.DataAccess
open Microsoft.AspNetCore.Http
open SmartRecipes.Business
open Microsoft.EntityFrameworkCore.ChangeTracking
open Generic

    [<RequireQualifiedAccess>]
    module Recipes =

        let index next ctx =
            json [] next ctx
            // authorize
            //>> Recipes.query.byAccount account
            // >> json

        let detail id =
            Generic.detail Recipes.query.withId id

        // Create TODO: make much more generic

        type createParameters = {
            name: string;
        }

        type createErrorMessage = 
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