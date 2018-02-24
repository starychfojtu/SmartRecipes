module Recipes
open SmartRecipes.Models
open System

type Recipes = {
    withId: Guid -> Recipe option
}

let sqlWithId id =
    None

let Recipes = RecipeQueriesSql    