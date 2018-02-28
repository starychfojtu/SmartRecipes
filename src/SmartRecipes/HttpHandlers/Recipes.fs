namespace SmartRecipes.Api.HttpHandlers

open Giraffe
open SmartRecipes.Models
open System
open SmartRecipes.DataAccess

    [<RequireQualifiedAccess>]
    module Recipes =
        open Microsoft.AspNetCore.Http
        open SmartRecipes.Business
        open Giraffe.GiraffeViewEngine

        let index next ctx =
            json [] next ctx
            // authorize
            //>> Recipes.query.byAccount account
            // >> json

        let detail id =
            Generic.detail Recipes.query.withId id

(*
        let create (ctx: HttpContext) (next: HttpFunc) = 
            let command = ctx.GetService<SmartRecipesContext>() |> Recipes.command
            let testAccount = {
                id = Guid.NewGuid();
                signInInfo = 
                {
                    email = "";
                    password = "";
                };
            }
            let recipe = Recipes.create "lasagne" testAccount
            Recipes.update recipe command

        let update (ctx: HttpContext) (next: HttpFunc) = 
            let command = ctx.GetService<SmartRecipesContext>() |> Recipes.command
            Recipes.update recipe command

        let delete (ctx: HttpContext) (next: HttpFunc) =
            let command = ctx.GetService<SmartRecipesContext>() |> Recipes.command
            Recipes.delete recipe command
*)