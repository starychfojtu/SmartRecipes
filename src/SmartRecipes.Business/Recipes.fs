namespace SmartRecipes.Business

open System
open SmartRecipes.Models
open SmartRecipes.DataAccess

[<RequireQualifiedAccess>]
module Recipes =

    let create name = { 
        id = Guid.NewGuid();
        name = name;
        creator = { id = Guid.Empty; signInInfo = {email=""; password=""}};
    }

    let add recipe =
        Recipes.command.create recipe

    let update recipe =
        Recipes.command.update recipe

    let delete recipe =
        Recipes.command.delete recipe