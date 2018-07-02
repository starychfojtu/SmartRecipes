namespace SmartRecipes.Business

open System
open SmartRecipes.Models
open SmartRecipes.DataAccess

[<RequireQualifiedAccess>]
module Recipes =

    let create name creatorId = { 
        id = Guid.NewGuid();
        name = name;
        creatorId = creatorId
    }

    let add recipe =
        Recipes.command.create recipe

    let update recipe =
        Recipes.command.update recipe

    let delete recipe =
        Recipes.command.delete recipe