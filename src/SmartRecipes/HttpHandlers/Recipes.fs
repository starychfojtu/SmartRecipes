namespace SmartRecipes.Api.HttpHandlers

open Giraffe
open SmartRecipes.Models
open System
open SmartRecipes.DataAccess

    [<RequireQualifiedAccess>]
    module Recipes =
        open Microsoft.AspNetCore.Http
        open SmartRecipes.Business

        let index (_:Unit) =
            //Recipes.query.byAccount account |> json
            json []

        let detail id: HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext)->
                let query =  ctx.GetService<SmartRecipesContext>() |> Recipes.query
                Generic.detail query.withId id next ctx

        let create (ctx: HttpContext) (next: HttpFunc) = 
            let command = ctx.GetService<SmartRecipesContext>() |> Recipes.command
            let testAccount = {
                id = Guid.NewGuid();
                recipes = [];
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
            let recipe = {
                id = Guid.NewGuid();
                name = "";
                creatorId = Guid.NewGuid();
                ingredients = [];
                steps = [];
            }
            Recipes.update recipe command

        let delete (ctx: HttpContext) (next: HttpFunc) =
            let command = ctx.GetService<SmartRecipesContext>() |> Recipes.command
            let recipe = {
                id = Guid.NewGuid();
                name = "";
                creatorId = Guid.NewGuid();
                ingredients = [];
                steps = [];
            }
            Recipes.delete recipe command