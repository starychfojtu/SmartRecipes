namespace SmartRecipes.Business

open System
open SmartRecipes.Models
open SmartRecipes.DataAccess

[<RequireQualifiedAccess>]
module Recipes =

    let create name (account: Account) = { 
        id = Guid.NewGuid();
        name = name;
        creatorId = account.id;
        ingredients = [];
        steps = [];
    }

    let add recipe (command: Recipes.Command) =
        command.create recipe

    let update recipe (command: Recipes.Command) =
        command.update recipe

    let delete recipe (command: Recipes.Command) =
        command.delete recipe

    module Repository =
        
        let withId id (query: Recipes.Query) =
            query.withId id