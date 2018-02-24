namespace SmartRecipes.DataAccess

[<RequireQualifiedAccess>]
module Recipes =
    open System
    open SmartRecipes.Models

    type Query = {
        withId: Guid -> Recipe option;
    }

    let sqlQuery = {
        withId = (fun id -> None)
    }

    let query = sqlQuery