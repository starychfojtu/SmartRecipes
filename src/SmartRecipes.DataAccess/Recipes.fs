namespace SmartRecipes.DataAccess

module Recipes =
    open System
    open SmartRecipes.Models

    type Query = {
        withId: Guid -> Recipe option;
    }

    let SqlQuery = {
        withId = (fun id -> None)
    }

    let query = SqlQuery