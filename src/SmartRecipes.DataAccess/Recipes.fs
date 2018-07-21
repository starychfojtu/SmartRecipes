namespace SmartRecipes.DataAccess

[<RequireQualifiedAccess>]
module Recipes =
    open DataAccess.Context
    open FSharpPlus.Data
    open System.Net.Http
    open System
    open Models.Recipe
    open Models.Account
    open DataAccess.Model
    open Models.Foodstuff
    
    let private toDb unit =
        match unit with
        | DbMetricUnit.Liter -> MetricUnit.Liter
        | DbMetricUnit.Gram -> MetricUnit.Gram
        | DbMetricUnit.Piece -> MetricUnit.Piece
    
    let private toModel (recipe: DbRecipe): Recipe = {
        id = RecipeId recipe.id;
        name = recipe.name;
        creatorId = AccountId recipe.creatorId
    }
    
    let getAll = Reader (fun (ctx: Context) ->
        ctx.Recipes
        |> Seq.toList
        |> Seq.map toModel
    )