namespace SmartRecipes.Api.HttpHandlers

open Giraffe
open SmartRecipes.Models

    [<RequireQualifiedAccess>]
    module Recipes =
        open System

        let index _ =
            let recipe = { id = Guid.NewGuid(); name = "Lasagne"; ingredients = [] }
            json recipe